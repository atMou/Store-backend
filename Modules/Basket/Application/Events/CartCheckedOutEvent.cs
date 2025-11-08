using Shared.Domain.Abstractions;
using Shared.Domain.Contracts.Cart;

namespace Basket.Application.Events;

internal record CartCheckedOutEvent(CartDto Dto) : IDomainEvent;

