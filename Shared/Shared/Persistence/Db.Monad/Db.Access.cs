using Shared.Application.Abstractions;

namespace Shared.Persistence.Db.Monad;

public static class Db
{
    public static Db<Ctx, A> AddEntity<Ctx, A, B>(
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
    public static Db<Ctx, A> AddEntity<Ctx, A>(
        A entity)
        where Ctx : DbContext where A : class
    {

        return
            from _2 in Db<Ctx>.lift(ctx =>
            {
                ctx.Set<A>().Add(entity);
                return unit;
            })
            select entity;
    }

    public static Db<Ctx, Unit> GetUpdateEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<A, Fin<A>> update)
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               from updatedA in update(a)
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select unit;
    }
    public static Db<Ctx, Unit> GetUpdateEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<A, Fin<A>> update,
        Func<QueryOptions<A>, QueryOptions<A>> fn)
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               from updatedA in update(a)
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select unit;
    }

    public static Db<Ctx, A> GetUpdateEntityA<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        Func<A, Fin<A>> update,
        NotFoundError error
    )
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               from updatedA in update(a)
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select updatedA;
    }
    public static Db<Ctx, A> GetUpdateEntityA<Ctx, A, B>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<A, B> doFn,
       params Func<A, B, Fin<A>>[] updates
    )
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))

               let b = doFn(a)
               from updatedA in updates.Aggregate(FinSucc(a), (current, func) => current.Bind(a => func(a, b)))
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select updatedA;
    }



    public static Db<Ctx, (A a, B b, C c)> GetUpdateEntityA<Ctx, A, B, C>(
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
    public static Db<Ctx, A> GetUpdateEntityA<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error,
        Func<QueryOptions<A>, QueryOptions<A>>? queryOptionsFn,
       params Func<A, Fin<A>>[] updates
    )
        where Ctx : DbContext where A : class, IAggregate
    {
        return queryOptionsFn.IsNotNull()
            ? (from a in Db<Ctx>.liftIO(async (ctx, e) =>
                    await ctx.Set<A>().WithQueryOptions(queryOptionsFn).FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               from updatedA in updates.Aggregate(FinSucc(a), (current, fn) => current.Bind(fn))
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select updatedA) :
            (from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().FirstOrDefaultAsync(predicate, e.Token))
             from _1 in when(a.IsNull(),
                 IO.fail<Unit>(error))
             from updatedA in updates.Aggregate(FinSucc(a), (current, fn) => current.Bind(fn))
             from _2 in Db<Ctx>.lift(ctx =>
             {
                 ctx.Set<A>().Entry(a).CurrentValues.SetValues(updatedA);
                 return unit;
             })
             select updatedA);
    }
    public static Db<Ctx, Unit> UpdateEntity<Ctx, A>(
        A entity,
        Func<A, Fin<A>> update)
        where Ctx : DbContext where A : class
    {
        return from updatedA in update(entity)
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(entity).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select unit;
    }
    public static Db<Ctx, Unit> UpdateEntity<Ctx, A>(
        A entity,
       params Func<A, Fin<A>>[] updates)
        where Ctx : DbContext where A : class
    {
        return from updatedA in updates.Aggregate(FinSucc(entity), (acc, fn) => acc.Bind(fn))
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Entry(entity).CurrentValues.SetValues(updatedA);
                   return unit;
               })
               select unit;
    }
    public static Db<Ctx, Unit> UpdateEntity<Ctx, A>(
        A entity,
        Func<A, A> update)
        where Ctx : DbContext where A : class
    {
        var updatedA = update(entity);
        return
            from _2 in Db<Ctx>.lift(ctx =>
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
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               let del = delete(a)
               from _2 in Db<Ctx>.lift(ctx =>
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
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               from _2 in Db<Ctx>.lift(ctx =>
               {
                   ctx.Set<A>().Remove(a);
                   return unit;
               })
               select a;
    }

    public static Db<Ctx, B> GetEntity<Ctx, A, B>(
        Expression<Func<A, bool>> predicate,
        Func<A, B> map,
        NotFoundError error)
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>()
                    .FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               select map(a);
    }

    public static Db<Ctx, A> GetEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        NotFoundError error)
        where Ctx : DbContext where A : class
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>()
                    .FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               select a;
    }

    public static Db<Ctx, B> GetEntity<Ctx, A, B>(
        Expression<Func<A, bool>> predicate,
        Func<QueryOptions<A>, QueryOptions<A>> fn,
        Func<A, B> map,
        NotFoundError error)
        where Ctx : DbContext where A : class, IAggregate
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(fn)
                    .FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               select map(a);
    }

    public static Db<Ctx, A> GetEntity<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        Func<QueryOptions<A>, QueryOptions<A>> fn,
        NotFoundError error)
        where Ctx : DbContext where A : class, IAggregate
    {
        return from a in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(fn)
                    .FirstOrDefaultAsync(predicate, e.Token))
               from _1 in when(a.IsNull(),
                   IO.fail<Unit>(error))
               select a;
    }

    public static Db<Ctx, List<A>> GetEntities<Ctx, A>(
        Expression<Func<A, bool>> predicate,
        Func<QueryOptions<A>, QueryOptions<A>>? fn)
        where Ctx : DbContext
        where A : class, IAggregate

    {
        return from lstA in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(fn).Where(predicate)
                    .ToListAsync(e.Token))
               select lstA;
    }

    public static Db<Ctx, List<A>> GetEntities<Ctx, A>(
        Expression<Func<A, bool>> predicate)
        where Ctx : DbContext
        where A : class, IAggregate

    {
        return from lstA in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().Where(predicate)
                    .ToListAsync(e.Token))
               select lstA;
    }

    public static Db<Ctx, PaginatedResult<B>> GetEntitiesWithPagination<Ctx, A, B, TQuery>
    (Expression<Func<A, bool>> predicate,
        TQuery query,
        Func<A, B> map,
        Func<QueryOptions<A>, QueryOptions<A>>? fn)
        where Ctx : DbContext
        where A : class, IAggregate
        where TQuery : IPagination

    {
        return from t in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(fn)
                    .Where(predicate)
                    .GroupBy(_ => 1)
                    .Select(g => new { TotalCount = g.Count(), Items = g.ToList() })
                    .FirstOrDefaultAsync(e.Token)
                    .Map(res => (res!.Items, res.TotalCount)))
               select new PaginatedResult<B>
               {
                   Items = t.Items.AsIterable().Map(map),
                   TotalCount = t.TotalCount,
                   PageSize = query.PageSize,
                   PageNumber = query.PageNumber
               };
    }


    public static Db<Ctx, PaginatedResult<B>> GetEntitiesWithPagination<Ctx, A, B, TQuery>
    (TQuery query,
        Func<A, B> map,
        Func<QueryOptions<A>, QueryOptions<A>>? fn)
        where Ctx : DbContext
        where A : class, IAggregate
        where TQuery : IPagination, IInclude

    {
        return from t in Db<Ctx>.liftIO(async (ctx, e) =>
                await ctx.Set<A>().WithQueryOptions(fn)
                    .GroupBy(_ => 1)
                    .Select(g => new { TotalCount = g.Count(), Items = g.ToList() })
                    .FirstOrDefaultAsync(e.Token)
                    .Map(res => (res!.Items, res.TotalCount)))
               select new PaginatedResult<B>
               {
                   Items = t.Items.AsIterable().Map(map),
                   TotalCount = t.TotalCount,
                   PageSize = query.PageSize,
                   PageNumber = query.PageNumber
               };
    }
}