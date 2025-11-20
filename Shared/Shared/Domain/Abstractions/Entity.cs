namespace Shared.Domain.Abstractions;
public abstract record Entity<TId>(TId Id) : IEntity<TId> where TId : IId
{

    public TId Id { get; } = Id;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

