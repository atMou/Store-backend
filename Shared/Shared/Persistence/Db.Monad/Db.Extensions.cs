// ReSharper disable InconsistentNaming

namespace Shared.Persistence.Db.Monad;

public static class DbExtensions
{
    public static Db<RT, A> As<A, RT>(this K<Db<RT>, A> db)
    {
        return (Db<RT, A>)db;
    }

    public static Fin<A> Run<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env)
    {
        return db.As().Run(rt, env);
    }

    public static Task<Fin<A>> RunAsync<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env)
    {
        return Try.lift(async () => await db.As().runDB
               .RunAsync(rt, env)
           ).Match(task => task, err => Task.FromResult(FinFail<A>(err)));

    }

    public static async Task<Fin<A>> RunSaveAsync<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env) where RT : DbContext
    {
        Fin<A> res = db.Run(rt, env);

        if (res.IsFail)
        {
            return res;
        }

        try
        {
            await rt.SaveChangesAsync(env.Token);
            return res;
        }
        catch (Exception e)
        {
            return FinFail<A>(e);
        }

    }

    public static async Task<Fin<A>> RunSaveAsyncT<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env) where RT : DbContext
    {
        Fin<A> res = db
            .Run(rt, env);
        await using var transaction = await rt.Database.BeginTransactionAsync(env.Token);
        return await res.Match<Task<Fin<A>>>(async a =>
        {
            await rt.SaveChangesAsync(env.Token);
            await transaction.CommitAsync(env.Token);
            return FinSucc(a);
        }, async e =>
        {
            await transaction.RollbackAsync(env.Token);
            return FinFail<A>(e);
        });

    }

    public static Task<Fin<B>> RaiseOnSuccess<A, B>(this Task<Fin<A>> ma, Func<A, Task<B>> onSuccess)
    {

        return ma.MapAsync(fa => fa.Match(async a =>
            {
                var b = await onSuccess.Invoke(a);
                return FinSucc(b);
            },
            e => Task.FromResult(FinFail<B>(e))));

    }

    public static Task<Fin<B>> RaiseOnSuccess<A, B>(this Task<Fin<A>> ma, Func<A, B> onSuccess)
    {

        return ma.Map(fa => fa.Match(
            a =>
            {
                var b = onSuccess.Invoke(a);
                return FinSucc(b);
            },
            Fail: FinFail<B>));

    }

    public static Task<Fin<A>> RaiseOnFail<A>(this Task<Fin<A>> ma, Func<Error, Task> onFail)
    {

        return ma.MapAsync(fa => fa.Match(
           a => Task.FromResult(FinSucc(a)),
            Fail: async e =>
            {
                await onFail.Invoke(e);
                return FinFail<A>(e);
            }));

    }

    public static Task<Fin<A>> RaiseOnFail<A>(this Task<Fin<A>> ma, Action<Error> onFail)
    {

        return ma.Map(fa => fa.Match(
            FinSucc,
            Fail: e =>
            {
                onFail.Invoke(e);
                return FinFail<A>(e);
            }));

    }

    public static Task<Fin<A>> RaiseBiEvent<A>(this Task<Fin<A>> ma, Action<A> onSuccess, Action<Error> onFail)
    {

        return ma.Map(fa => fa.Match(
             a =>
            {
                onSuccess.Invoke(a);
                return FinSucc(a);
            },
            Fail: e =>
            {
                onFail.Invoke(e);
                return FinFail<A>(e);
            }));

    }

    public static async Task<Fin<A>> RaiseBiEvent<A>(this Task<Fin<A>> ma, Func<A, Task> onSuccess, Func<Error, Task> onFail)
    {

        return await ma.MapAsync(fa => fa.Match(
            async a =>
            {
                await onSuccess.Invoke(a);
                return FinSucc(a);
            },
            Fail: async e =>
            {
                await onFail.Invoke(e);
                return FinFail<A>(e);
            }));

    }

    public static Db<RT, C> SelectMany<A, B, C, RT>(this Fin<A> ma, Func<A, Db<RT, B>> bind, Func<A, B, C> project) =>
        ma.Match(
            Succ: a => bind(a).Map(b => project(a, b)),
            Fail: Db<RT, C>.Fail
        );


    public static Db<RT, B> Apply<RT, A, B>(this Db<RT, Func<A, B>> mf, Db<RT, A> ma)
    {
        return mf.As().Bind(f => ma.Map(f));
    }
    public static K<Db<RT>, B> Apply<RT, A, B>(this Db<RT, Func<A, B>> mf, K<Db<RT>, A> ma)
    {
        return mf.Bind(f => ma.As().Map(f));
    }
    public static Db<RT, B> Apply<RT, A, B>(this K<Db<RT>, Func<A, B>> mf, Db<RT, A> ma)
    {
        return mf.As().Bind(f => ma.Map(f));
    }

    public static Db<RT, C> Apply<RT, A, B, C>(this (Db<RT, A>, Db<RT, B>) items, Func<A, B, C> f)
    {
        return f.Map(items.Item1).Apply(items.Item2);
    }

    public static Db<RT, C> Apply<RT, A, B, C>(this (K<Db<RT>, A>, K<Db<RT>, B>) items, Func<A, B, C> f)
    {
        return f.Map(items.Item1).Apply(items.Item2).As();
    }

    public static Db<RT, C> Apply<RT, A, B, C>(this (Fin<A>, K<Db<RT>, B>) items, Func<A, B, C> f)
    {
        return Apply((Db<RT>.lift(items.Item1), items.Item2), f);
    }



    public static Db<RT, D> Apply<RT, A, B, C, D>(this (Db<RT, A>, Db<RT, B>, Db<RT, C>) items, Func<A, B, C, D> f)
    {
        return f.Map(items.Item1).Apply(items.Item2).Apply(items.Item3).As();
    }


    public static Db<RT, E> Apply<RT, A, B, C, D, E>(this (Db<RT, A>, Db<RT, B>, Db<RT, C>, Db<RT, D>) items, Func<A, B, C, D, E> f)
    {
        return f.Map(items.Item1).Apply(items.Item2).Apply(items.Item3).Apply(items.Item4).As();
    }


    public static Db<RT, D> Apply<RT, A, B, C, D>(this (K<Db<RT>, A>, K<Db<RT>, B>, K<Db<RT>, C>) items, Func<A, B, C, D> f)
    {
        return f.Map(items.Item1).Apply(items.Item2).Apply(items.Item3).As();
    }

    public static IO<C> SelectMany<A, B, C>(this Fin<A> ma, Func<A, IO<B>> bind, Func<A, B, C> project)
    {
        return ma.Match(
            a => bind(a).Map(b => project(a, b)),
         IO.fail<C>
        );
    }


    public static IO<C> SelectMany<A, B, C>(this IO<A> ma, Func<A, Fin<B>> bind, Func<A, B, C> project)
    {
        return ma.Bind(a => bind(a).Match(
            b => IO.lift(() => project(a, b)), IO.fail<C>
        ));
    }


    //public static IO<C> SelectMany<A, B, C>(this IO<Unit> ma, Func<Unit, Fin<B>> bind, Func<Unit, B, C> project)
    //{
    //    return ma.Bind(x => bind(x).Match(
    //        b => IO.pure(project(x, b)), IO.fail<C>
    //    ));
    //}


    //public static Fin<C> SelectMany<A, B, C>(this IO<Unit> ma, Func<Unit, Fin<B>> bind, Func<Unit, B, C> project)
    //{
    //    Fin<B> run = ma.Bind(bind).Run(EnvIO.New(null, CancellationToken.None));
    //}



}

