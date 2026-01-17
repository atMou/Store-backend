namespace Shared.Domain.Abstractions;

public interface IEntity<TId> where TId : IId
{
    public TId Id { get; }
}


public interface IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}