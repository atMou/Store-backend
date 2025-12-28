using Shared.Application.Abstractions;

namespace Shared.Application.Features.Payment.Events;

public record PaymentFulfilledIntegrationEvent : IntegrationEvent
{
	public PaymentId PaymentId { get; init; }
	public OrderId OrderId { get; init; }
	public UserId UserId { get; init; }
	public CartId CartId { get; init; }
	public string PaymentTransactionId { get; init; }
	public DateTime PaymentPaidAt { get; init; }
}
