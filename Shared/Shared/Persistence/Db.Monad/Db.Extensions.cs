// ReSharper disable InconsistentNaming

namespace Shared.Persistence.Db.Monad;

public static class DbExtensions
{
    public static Db<RT, A> As<A, RT>(this K<Db<RT>, A> db)
    {
        return (Db<RT, A>)db;
    }

    public static Fin<A> Run<RT, A>(this Db<RT, A> db, RT env)
    {
        return db.runDB
            .Run(env);
    }


    public static Fin<A> Run<RT, A>(this K<Db<RT>, A> db, RT rt)
    {
        return db.As().runDB
            .Run(rt);
    }

    public static Fin<A> Run<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env)
    {
        return db.As().runDB
            .Run(rt, env);
    }

    public static Task<Fin<A>> RunAsync<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env)
    {
        return db.As().runDB
            .RunAsync(rt, env);
    }

    public static async Task<Fin<A>> RunSave<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env) where RT : DbContext
    {
        Fin<A> res = db.As().runDB
            .Run(rt, env);
        return await res.Match(async a =>
        {
            await rt.SaveChangesAsync(env.Token);
            return FinSucc(a);
        }, e => Task.FromResult(FinFail<A>(e)));

    }

    public static async Task<Fin<A>> RunSaveT<RT, A>(this K<Db<RT>, A> db, RT rt, EnvIO env) where RT : DbContext
    {
        Fin<A> res = db.As().runDB
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

    public static Task<Fin<A>> RaiseOnSuccess<A>(this Task<Fin<A>> ma, Action<A> onSuccess)
    {

        return ma.Map(fa => fa.Match(
            a =>
            {
                onSuccess.Invoke(a);
                return FinSucc(a);
            },
            Fail: FinFail<A>));

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


}

