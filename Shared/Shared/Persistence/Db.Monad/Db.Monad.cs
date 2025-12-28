namespace Shared.Persistence.Db.Monad;

public partial class Db<RT> : Deriving.Monad<Db<RT>, Eff<RT>>, Deriving.Alternative<Db<RT>, Eff<RT>>,
	Deriving.Readable<Db<RT>, RT, Eff<RT>>,
	Fallible<Db<RT>>,
	MonadIO<Db<RT>>
{
	public static K<Db<RT>, A> Fail<A>(Error error)
	{
		return new Db<RT, A>(LanguageExt.Eff<RT, A>.Fail(error));
	}

	public static K<Db<RT>, A> Catch<A>(K<Db<RT>, A> fa, Func<Error, bool> Predicate,
		Func<Error, K<Db<RT>, A>> Fail)
	{
		return new Db<RT, A>(fa.As().runDB.Catch(Predicate, error => Fail(error).As().runDB).As());
	}

	public static K<Eff<RT>, A> Transform<A>(K<Db<RT>, A> fa)
	{
		return fa.As().runDB;
	}

	public static K<Db<RT>, A> CoTransform<A>(K<Eff<RT>, A> fa)
	{
		return new Db<RT, A>(fa.As());
	}

	static K<Db<RT>, A> MonadIO<Db<RT>>.LiftIO<A>(IO<A> ma)
	{
		return new Db<RT, A>(LanguageExt.Eff<RT, A>.LiftIO(ma));
	}
}