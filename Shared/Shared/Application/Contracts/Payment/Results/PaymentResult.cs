namespace Shared.Application.Contracts.Payment.Results;
public record PaymentResult
{
    public Guid PaymentId { get; init; }
    public decimal Total { get; init; }
    public decimal Tax { get; init; }
    public string Status { get; init; }
    public DateTime? PaidAt { get; init; }
    public DateTime? RefundedAt { get; init; }
    public DateTime? FailedAt { get; init; }
    public string PaymentMethod { get; init; }
}
