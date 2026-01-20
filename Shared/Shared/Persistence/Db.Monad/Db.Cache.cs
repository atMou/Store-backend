using Shared.Application.Contracts;

namespace Shared.Persistence.Db.Monad;
public static partial class Db
{
    public static Db<Ctx, PaginatedResult<A>> GetCachedPaginatedEntities<Ctx, A>(string key)
    where A : class
    where Ctx : IDistributedCache
    {
        return from s in Db<Ctx>.liftIO
            (async (ctx, e)
                => await ctx.GetStringAsync(key, e.Token))
               from _ in when(s.IsNull(), Db<Ctx>.fail<Unit>(NotFoundError.New("")))
               from entity in DeserializePaginatedEntity<A>(s)
               select entity;

    }

    public static Db<Ctx, A> GetCachedEntity<Ctx, A>(string key)
        where A : class
         where Ctx : IDistributedCache
    {
        return from s in Db<Ctx>.liftIO
             (async (ctx, e)
                 => await ctx.GetStringAsync(key, e.Token))
               from _ in when(s.IsNull(), Db<Ctx>.fail<Unit>(NotFoundError.New("")))
               from entity in DeserializeEntity<A>(s)
               select entity;

    }
    public static Db<Ctx, Unit> SetCachedEntity<Ctx, A>(
        string key,
        A entity,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null)
        where Ctx : IDistributedCache
        where A : class
    {
        return from jsonString in SerializeEntity(entity)
               from _ in Db<Ctx>.liftIO(async (cache, e) =>
               {
                   var options = new DistributedCacheEntryOptions();

                   if (absoluteExpiration.HasValue)
                       options.AbsoluteExpirationRelativeToNow = absoluteExpiration.Value;

                   if (slidingExpiration.HasValue)
                       options.SetSlidingExpiration(slidingExpiration.Value);

                   await cache.SetStringAsync(key, jsonString, options, e.Token);
                   return unit;
               })
               select unit;
    }

    public static Db<Ctx, Unit> SetPaginatedCachedEntities<Ctx, A>(
        string key,
        PaginatedResult<A> pEntities,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null)
        where Ctx : IDistributedCache
        where A : class
    {
        return from jsonString in SerializePaginatedEntities(pEntities)
               from _ in Db<Ctx>.liftIO(async (cache, e) =>
               {
                   var options = new DistributedCacheEntryOptions();

                   if (absoluteExpiration.HasValue)
                       options.AbsoluteExpirationRelativeToNow = absoluteExpiration.Value;

                   if (slidingExpiration.HasValue)
                       options.SetSlidingExpiration(slidingExpiration.Value);

                   await cache.SetStringAsync(key, jsonString, options, e.Token);
                   return unit;
               })
               select unit;
    }
    public static Db<Ctx, Unit> RemoveCachedEntity<Ctx>(string key)
        where Ctx : IDistributedCache
    {
        return Db<Ctx>.liftIO(async (cache, e) =>
        {
            await cache.RemoveAsync(key, e.Token);
            return unit;
        });
    }
    private static Fin<A> DeserializeEntity<A>(string json) where A : class
    {
        try
        {
            var entity = JsonSerializer.Deserialize<A>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });

            return entity is not null
                ? FinSucc(entity)
                : FinFail<A>(Error.New("DESERIALIZATION_ERROR Failed to deserialize cached entity."));
        }
        catch (Exception ex)
        {
            return FinFail<A>(InvalidOperationError.New(ex.Message));
        }
    }

    private static Fin<PaginatedResult<A>> DeserializePaginatedEntity<A>(string json) where A : class
    {
        try
        {
            var entity = JsonSerializer.Deserialize<PaginatedResult<A>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });

            return entity is not null
                ? FinSucc(entity)
                : FinFail<PaginatedResult<A>>(Error.New("DESERIALIZATION_ERROR Failed to deserialize cached entity."));
        }
        catch (Exception ex)
        {
            return FinFail<PaginatedResult<A>>(InvalidOperationError.New(ex.Message));
        }
    }

    private static Fin<string> SerializePaginatedEntities<A>(PaginatedResult<A> pEntities) where A : class
    {
        try
        {
            var json = JsonSerializer.Serialize(pEntities, new JsonSerializerOptions
            {
                WriteIndented = false,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });

            return FinSucc(json);
        }
        catch (Exception ex)
        {
            return FinFail<string>(Error.New(ex.Message));
        }
    }

    private static Fin<string> SerializeEntity<A>(A entity) where A : class
    {
        try
        {
            var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                WriteIndented = false,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });

            return FinSucc(json);
        }
        catch (Exception ex)
        {
            return FinFail<string>(Error.New(ex.Message));
        }
    }
}
