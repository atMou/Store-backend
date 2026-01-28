namespace Shared.Persistence.Db.Monad;
using LanguageExt;

public record Db<RT, A>(Eff<RT, A> runDB) : Fallible<Db<RT, A>, Db<RT>, Error, A>
{
    public static implicit operator Db<RT, A>(Pure<A> ma) =>
        Pure(ma.Value);

    public static implicit operator Db<RT, A>(Fail<Error> ma) =>
        Fail(ma.Value);


    ///  lifting 
    public static Db<RT, A> Pure(A x) =>
        new(Eff<RT, A>.Pure(x));

    public static Db<RT, A> Lift(Func<RT, Either<Error, A>> f) =>
            new(Eff<RT, A>.Lift(f));


    public static Db<RT, A> Lift(Func<RT, Fin<A>> f) =>
        new(Eff<RT, A>.Lift(f));

    public static Db<RT, A> Lift(Func<RT, A> f) =>
            new(Eff<RT, A>.Lift(f));

    //public static Db<RT, A> Lift(Func<RT,Option<A> >f) =>

    /// lifting IO
    public static Db<RT, A> LiftIO(IO<A> ma) =>
        new(Eff<RT, A>.LiftIO(ma));

    public static Db<RT, A> LiftIO(Func<RT, Task<A>> f) =>
        new(Eff<RT, A>.LiftIO(f));

    public static Db<RT, A> LiftIO(Func<RT, EnvIO, Task<A>> f) =>
        new(Eff<RT, A>.LiftIO(rt => IO.liftAsync(envIo => f(rt, envIo))));

    public static Db<RT, A> LiftIO(Func<RT, EnvIO, ValueTask<A>> f) =>
        new(Eff<RT, A>.LiftIO(rt => IO.liftVAsync(envIo => f(rt, envIo))));

    public static Db<RT, A> LiftIO(Func<RT, Task<Fin<A>>> f) =>
        new(Eff<RT, A>.LiftIO(f));

    public static Db<RT, A> LiftIO(Func<RT, Task<Option<A>>> f, Error fail) =>
        new(Eff<RT, A>.LiftIO(rt => f(rt).Map(o => o.ToFin(fail))));

    public static Db<RT, A> LiftIO(Func<RT, IO<A>> f) =>
        new(Eff<RT, A>.LiftIO(f));

    public static Db<RT, A> Lift(Func<Either<Error, A>> f) =>
        new(Eff<RT, A>.Lift(_ => f()));

    public static Db<RT, A> LiftEff(Eff<RT, A> ma) =>
        new(ma);

    public static Db<RT, A> Lift(Func<Fin<A>> f) =>
        new(Eff<RT, A>.Lift(f));

    public static Db<RT, A> Lift(Func<A> f) =>
            new(Eff<RT, A>.Lift(_ => f()));

    public static Db<RT, A> LiftIO(Func<Task<A>> f) =>
        new(Eff<RT, A>.LiftIO(_ => f()));

    public static Db<RT, A> LiftIO(Func<Task<Fin<A>>> f) =>
        new(Eff<RT, A>.LiftIO(_ => f()));

    public static Db<RT, A> Fail(Error error) =>
        new(Eff<RT, A>.Fail(error));

    ///  Map and Bind
    public Db<RT, B> Map<B>(Func<A, B> f) =>
        new(runDB.Map(f));

    public Db<RT, B> Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        Map(Succ).Catch(Fail).As();

    public Db<RT, A> IfFail(Func<Error, K<Db<RT>, A>> Fail) =>
       this.Catch(Fail).As();

    public Db<RT, A> IfFail(Func<Error, Db<RT, A>> Fail) =>
        this.Catch(Fail).As();

    public Db<RT, A> IfFail(Func<Error, A> Fail) =>
        Match(Prelude.identity, Fail);

    public Db<RT, A> MapFail(Func<Error, Error> f) =>
        new(runDB.MapFail(f));

    public Db<RT, B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
      Map(Succ).Catch(Fail).As();

    // Bindings
    public Db<RT, B> Bind<B>(Func<A, K<Db<RT>, B>> f) =>
        new(runDB.Bind(a => f(a).As().runDB));

    public Db<RT, B> Bind<B>(Func<A, Db<RT, B>> f) =>
        new(runDB.Bind(a => f(a).As().runDB));

    public Db<RT, B> Bind<B>(Func<A, IO<B>> f) =>
            Bind(a => Db<RT>.liftIO(f(a))).As();

    public Db<RT, B> Bind<B>(Func<A, Eff<RT, B>> f) =>
        Bind(a => new Db<RT, B>(f(a)));

    public Db<RT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(a => f(a).Value);

    public Db<RT, A> Bind(Func<A, Fail<Error>> f) =>
        Bind(a => Fail(f(a).Value));

    public Db<RT, A> BindFail(Func<Error, Db<RT, A>> f) =>
        this | @catch(err => f(err));

    public Db<RT, B> Select<B>(Func<A, B> f) =>
        Map(f);

    public Db<RT, C> SelectMany<B, C>(Func<A, K<Db<RT>, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));

    public Db<RT, C> SelectMany<B, C>(Func<A, Db<RT, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));


    public Db<RT, C> SelectMany<B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));

    public Db<RT, C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Match(b => Db<RT>.pure(project(a, b)), Db<RT>.fail<C>));

    public Db<RT, C> SelectMany<B, C>(Func<A, K<Eff<RT>, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).As().Map(b => project(a, b)));

    public Db<RT, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(a => Db<RT>.liftIO(bind(a)), project);
    public Db<RT, C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        SelectMany(a => Db<RT>.liftIO<B>(bind(a).As()), project);

    public Db<RT, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(a => project(a, bind(a).Value));

    public Db<RT, C> SelectMany<B, C>(Func<A, Fail<Error>> bind, Func<A, B, C> project) =>
       SelectMany(a => Db<RT>.fail<B>(bind(a).Value), project);

    public static implicit operator Db<RT, A>(IO<A> ma) => Db<RT>.liftIO(ma);

    public static implicit operator Db<RT, A>(Eff<RT, A> ma) =>
     LiftEff(ma);

    public static implicit operator Db<RT, A>(Error ma) => Db<RT>.liftIO(IO<A>.Fail(ma));

    public static implicit operator Db<RT, A>(Fail<string> ma) => Db<RT>.liftIO(IO<A>.Fail((Error)ma.Value));

    public static Db<RT, A> operator >>(Db<RT, A> ma, K<Db<RT>, A> mb) =>
        ma.Bind(_ => mb);

    public static Db<RT, A> operator >>(Db<RT, A> ma, Db<RT, A> mb) =>
        ma.Bind(_ => mb);

    public static Db<RT, A> operator >>(Db<RT, A> ma, K<Db<RT>, Unit> mb) =>
        ma.Bind(a => mb.Map(_ => a));

    public static Db<RT, A> operator >>(Db<RT, A> ma, Db<RT, Unit> mb) =>
        ma.Bind(a => mb.Map(_ => a));

    public static Db<RT, A> operator >>(Db<RT, A> ma, K<IO, A> mb) =>
        ma.Bind(_ => mb.As());

    public static K<Db<RT>, RT> Ask() =>
        Readable.ask<Db<RT>, RT>();

    public static K<Db<RT>, A> Asks(Func<RT, A> f) =>
        Readable.asks<Db<RT>, RT, A>(f);

    public static K<Db<RT>, A> Local(Func<RT, RT> f, K<Db<RT>, A> ma) =>
        Readable.local(f, ma);

    public static Db<RT, A> operator |(Db<RT, A> lhs, Db<RT, A> rhs) =>
        lhs.Choose(rhs).As();

    static Db<RT, A> Fallible<Db<RT, A>, Db<RT>, Error, A>.operator |(K<Db<RT>, A> lhs, Db<RT, A> rhs) =>
        lhs.Choose(rhs).As();

    static Db<RT, A> Fallible<Db<RT, A>, Db<RT>, Error, A>.operator |(Db<RT, A> lhs, K<Db<RT>, A> rhs) =>
        lhs.Choose(rhs).As();

    public static Db<RT, A> operator |(Db<RT, A> lhs, Pure<A> rhs) =>
        lhs.Choose(Db<RT>.pure(rhs.Value)).As();

    public static Db<RT, A> operator |(Db<RT, A> lhs, Fail<Error> rhs) =>
        lhs.Catch(rhs).As();

    public static Db<RT, A> operator |(Db<RT, A> ma, Error error) =>
        ma.Catch(error).As();

    public static Db<RT, A> operator |(Db<RT, A> ma, A value) =>
        ma.Catch(value).As();

    public static Db<RT, A> operator |(Db<RT, A> lhs, CatchM<Error, Db<RT>, A> rhs) =>
        lhs.Catch(rhs).As();

    public Fin<A> Run(RT rt) => runDB.Run(rt);

    public Fin<A> Run(RT rt, EnvIO envIo) => runDB.Run(rt, envIo);
    
    public async Task<Fin<A>> RunAsync(RT rt, EnvIO envIo) => await runDB.RunAsync(rt, envIo);

}
