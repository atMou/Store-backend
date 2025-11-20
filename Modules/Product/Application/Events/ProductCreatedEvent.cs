namespace Product.Application.Events;

public record ProductCreatedDomainEvent(Domain.Models.Product Product) : IDomainEvent
{

}
