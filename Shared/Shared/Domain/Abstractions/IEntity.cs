namespace Shared.Domain.Abstractions;

public interface IEntity<TId>
{
    public TId Id { get; set; }
}

public interface IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}