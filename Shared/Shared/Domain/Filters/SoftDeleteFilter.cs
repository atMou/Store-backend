namespace Shared.Domain.Filters;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
}

public class SoftDeleteFilter<TEntity> : IGlobalFilter<TEntity>
    where TEntity : class, ISoftDeletable
{
    public Expression<Func<TEntity, bool>> FilterExpression => entity => !entity.IsDeleted;

    public string Name => "SoftDelete";

    public bool IsDefaultEnabled => true;
}
