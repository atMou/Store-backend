namespace Shared.Domain.Abstractions;

/// <summary>
/// Represents a global query filter that can be applied to entities
/// </summary>
/// <typeparam name="TEntity">The entity type to filter</typeparam>
public interface IGlobalFilter<TEntity> where TEntity : class
{

    Expression<Func<TEntity, bool>> FilterExpression { get; }

    string Name { get; }

    bool IsDefaultEnabled { get; }
}
