// ReSharper disable InconsistentNaming

namespace Shared.Persistence.Db.Monad;

public partial class Db<RT>
{
	public static Db<RT, RT> dbEnv => new(runtime<RT>());

	public static Db<RT, A> fail<A>(Error error) =>
		new(LanguageExt.Eff<RT, A>.Fail(error));

	public static Db<RT, A> lift<A>(Func<RT, Either<Error, A>> f) =>
		Db<RT, A>.Lift(f);


	public static Db<RT, A> lift<A>(Func<RT, Fin<A>> f) =>
		Db<RT, A>.Lift(f);


	public static Db<RT, A> lift<A>(Fin<A> ma) =>
		Db<RT, A>.Lift(() => ma);

	public static Db<RT, A> lift<A>(A a) =>
		Db<RT, A>.Lift(() => a);

	public static Db<RT, A> lift<A>(Func<RT, A> f) =>
		Db<RT, A>.Lift(f);

	public static Db<RT, A> pure<A>(A value) =>
		new(LanguageExt.Eff<RT, A>.Pure(value));

	public static Db<RT, A> liftIO<A>(IO<A> ma) =>
		Db<RT, A>.LiftIO(ma);

	public static Db<RT, A> liftIO<A>(Func<RT, Task<A>> f) =>
		Db<RT, A>.LiftIO(f);

	public static Db<RT, A> liftIO<A>(Func<RT, EnvIO, Task<A>> f) =>
		Db<RT, A>.LiftIO(f);

	public static Db<RT, A> liftIO<A>(Func<RT, Task<Fin<A>>> f) =>
		Db<RT, A>.LiftIO(f);

	public static Db<RT, A> liftIO<A>(Func<RT, Task<Option<A>>> f, Error fail) =>
		Db<RT, A>.LiftIO(f, fail);

	public static Db<RT, A> liftIO<A>(Func<RT, IO<A>> f) =>
		Db<RT, A>.LiftIO(f);

	public static Db<RT, A> lift<A>(Func<Either<Error, A>> f) =>
		Db<RT, A>.Lift(_ => f());


	public static Db<RT, A> liftEff<A>(Eff<RT, A> ma) =>
		new(ma);

	public static Db<RT, A> lift<A>(Func<Fin<A>> f) =>
		Db<RT, A>.Lift(f);

	public static Db<RT, A> lift<A>(Func<A> f) =>
		Db<RT, A>.Lift(f);

	public static Db<RT, A> liftIO<A>(Func<Task<A>> f) =>
		Db<RT, A>.LiftIO(f);

	public static Db<RT, A> liftIO<A>(Func<Task<Fin<A>>> f) =>
		Db<RT, A>.LiftIO(f);


	public static Db<RT, A> asks<A>(Func<RT, A> f) =>
		Readable.asks<Db<RT>, RT, A>(f).As();

	public static Db<RT, RT> ask() =>
		Readable.ask<Db<RT>, RT>().As();

	public static Db<RT, A> local<A>(Func<RT, RT> f, K<Db<RT>, A> ma) =>
		Readable.local(f, ma).As();

	public static Db<RT, A> local<A>(Func<RT, RT> f, Db<RT, A> ma) =>
		Readable.local(f, ma).As<A, RT>();
}
