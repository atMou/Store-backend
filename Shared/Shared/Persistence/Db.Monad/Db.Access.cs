using Shared.Application.Contracts;

namespace Shared.Persistence.Db.Monad;

public static partial class Db
{

    public static Db<Ctx, PaginatedResult<B>> GetEntitiesWithPagination<Ctx, A, B, TQuery>
    (
        Expression<Func<A, bool>>? predicate,
        Func<QueryOptions<A>, QueryOptions<A>>? fn,
        TQuery query,
        Func<A, B> map
        )
        where Ctx : DbContext
        where A : class, IAggregate
        where TQuery : IPagination

    {
        return from result in Db<Ctx>.liftIO(async (ctx, e) =>
            {
                var totalCount = await ctx.Set<A>().CountAsync(e.Token);
                var items = predicate switch
                {
                    null => await ctx.Set<A>()
                        .WithQueryOptions(fn).ToListAsync(e.Token),

                    _ => await ctx.Set<A>()
                        .Where(predicate)
                        .WithQueryOptions(fn).ToListAsync(e.Token)
                };
                return (Items: items, TotalCount: totalCount);
            })
               select new PaginatedResult<B>
               {
                   Items = result.Items.Select(map),
                   TotalCount = result.TotalCount,
                   PageSize = query.PageSize,
                   PageNumber = query.PageNumber
               };
    }
    
       public static Db<Ctx, A> GetEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<QueryOptions<A>, QueryOptions<A>>? fn = null
    )
        where Ctx : DbContext where A : class, IAggregate
    {

        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(fn)
                    .FirstOrDefaultAsync(predicate, e.Token))
               from _ in when(a.IsNull(),
                   IO.fail<Unit>(error))
               select a;
    }

    public static Db<Ctx, List<A>> GetEntities<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        Func<QueryOptions<A>, QueryOptions<A>>? fn = null)
        where Ctx : DbContext
        where A : class, IAggregate

    {
        return from lstA in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().Where(predicate).WithQueryOptions(fn)
                    .ToListAsync(e.Token))
               select lstA;
    }

    public static Db<Ctx, Unit> GetUpdateEntities<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        Func<QueryOptions<A>, QueryOptions<A>>? queryOptions,
        params Func<A, Fin<A>>[] updates)
        where Ctx : DbContext
        where A : class, IAggregate

    {
        var db = from lstA in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(queryOptions).Where(predicate)
                    .ToListAsync(e.Token))
                 from x in lstA.AsIterable()
                     .Traverse(a => updates.Aggregate(FinSucc(a),
                         (current, func) => current.Bind(func))).As()
                 let entities = x.AsEnumerable()
                 from __ in Db<Ctx>.lift(ctx =>
                 {
                     ctx.Set<A>().UpdateRange(entities);
                     return unit;
                 })
                 select unit;
        return db;
    }

    public static Db<Ctx, A> AddEntityIfNotExists<Ctx, A, B>(
        Expression<Func<A, bool>> predicate,
        ConflictError error,
        B createDto,
        Func<B, Fin<A>> create,
        params Func<A, A>[] updates)
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNotNull(),
                   IO.fail<Unit>(error))
               from res in create(createDto).Map(a => updates.Aggregate(a, (current, fn) => fn(current)))
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Add(res);
                   return unit;
               })
               select res;
    }

    public static Db<Ctx, A> AddEntity<Ctx, A, B>(
        B createDto,
        Func<B, Fin<A>> create,
        params Func<A, IO<A>>[] updates)
        where Ctx : DbContext where A : class
    {
        return
            (from res in create(createDto)
             from res2 in updates.AsIterable().Fold(IO.pure(res), (io, fn) => io.Bind(a => fn(a)))
             from _2 in Db<Ctx>.lift(ctx =>
             {
                 ctx.Set<A>().Add(res2);
                 return unit;
             })
             select res2).As();
    }

    public static Db<Ctx, A> AddEntity<Ctx, A>(
        A entity)
        where Ctx : DbContext where A : class, IAggregate
    {
        return
            from _2 in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Add(entity);
                return unit;
            })
            select entity;
    }

    public static Db<Ctx, A> AddEntity<Ctx, A>(
        Fin<A> ma)
        where Ctx : DbContext where A : class
    {
        return
            from entity in ma
            from _2 in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Add(entity);
                return unit;
            })
            select entity;
    }

    public static Db<Ctx, (A a, B b, C c)> GetUpdateEntity<Ctx, A, B, C>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        (Func<A, B> First, Func<A, C> Second) doFns,
        params Func<A, B, C, Fin<A>>[] updates
    )
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               let b = doFns.First(a)
               let c = doFns.Second(a)
               from updatedA in updates.Aggregate(FinSucc(a), (current, func) => current.Bind(a => func(a, b, c)))
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select (updatedA, b, c);
    }

    public static Db<Ctx, A> GetUpdateEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<QueryOptions<A>, QueryOptions<A>>? fn = null,
        params Func<A, Fin<A>>[] updates
    )
        where Ctx : DbContext where A : class, IAggregate
    {
        return
            from a in fn.IsNull()
                ? Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
                : Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().WithQueryOptions(fn)
                        .FirstOrDefaultAsync(predicate, e.Token))
            from _1 in when(a.IsNull(),
                IO.fail<Unit>(error))
            from updatedA in updates.Aggregate(FinSucc(a), (current, func) => current.Bind(func))
            from _2 in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Update(updatedA);
                return unit;
            })
            select updatedA;
    }

    public static Db<Ctx, A> GetUpdateEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<QueryOptions<A>, QueryOptions<A>>? fn = null,
        params Func<A, A>[] updates
    )
        where Ctx : DbContext where A : class, IAggregate
    {
        return
            from a in fn.IsNull()
                ? Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
                : Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().WithQueryOptions(fn)
                        .FirstOrDefaultAsync(predicate, e.Token))
            from _ in when(a.IsNull(),
                IO.fail<Unit>(error))
            let updatedA = updates.AsIterable().Fold(a, (current, func) => func(current))
            from __ in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Update(updatedA);
                ctx.ChangeTracker.DetectChanges();
                var entry = ctx.Entry(updatedA);
                return unit;
            })
            select updatedA;
    }

    public static Db<Ctx, A> GetUpdateEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<QueryOptions<A>, QueryOptions<A>>? fn = null,
        params Func<A, IO<A>>[] updates
    )
        where Ctx : DbContext where A : class, IAggregate
    {
        return
            from a in fn.IsNull()
                ? Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
                : Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().WithQueryOptions(fn)
                        .FirstOrDefaultAsync(predicate, e.Token))
            from _ in when(a.IsNull(),
                IO.fail<Unit>(error))
            from updatedA in updates.AsIterable().Fold(IO.pure(a), (current, func) => current.Bind(a => func(a)))
            from __ in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Update(updatedA);
                return unit;
            })
            select updatedA;
    }

    public static Db<Ctx, A> GetUpdateEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<QueryOptions<A>, QueryOptions<A>>? fn,
        Func<A, Fin<A>> update1,
        Func<A, IO<A>> update2
    )
        where Ctx : DbContext where A : class, IAggregate
    {
        return
            from a in fn.IsNull()
                ? Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
                : Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().WithQueryOptions(fn)
                        .FirstOrDefaultAsync(predicate, e.Token))
            from _ in when(a.IsNull(),
                IO.fail<Unit>(error))
            from a1 in update1(a)
            from element in update2(a1)
            from __ in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Update(element);
                return unit;
            })
            select element;
    }

    public static Db<Ctx, Unit> UpdateEntity<Ctx, A>(
        A entity,
        params Func<A, Fin<A>>[] updates)
        where Ctx : DbContext where A : class
    {
        return from updatedA in updates.Aggregate(FinSucc(entity), (acc, fn) => acc.Bind(fn))
               from __ in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(entity).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select unit;
    }

    public static Db<Ctx, A> SoftDeleteEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        Func<A, A> delete, NotFoundError error)
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _ in when(a.IsNull(),
                   IO.fail<Unit>(error))
               let del = delete(a)
               from __ in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(del);
                   return unit;
               })
               select a;
    }

    public static Db<Ctx, A> HardDeleteEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error)
        where Ctx : DbContext where A : class, IAggregate
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>()
                    .FirstOrDefaultAsync(predicate, e.Token))
               from _ in when(a.IsNull(),
                   IO.fail<Unit>(error))
               from __ in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Remove(a);
                   return unit;
               })
               select a;
    }


 
}