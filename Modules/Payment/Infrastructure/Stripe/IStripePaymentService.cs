namespace Payment.Infrastructure.Stripe;

public interface IStripePaymentService
{
    Task<Fin<StripePaymentIntentResponse>> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        Guid orderId,
        Guid userId,
        string customerEmail,
        CancellationToken cancellationToken = default);

    Task<Fin<StripePaymentIntentResponse>> GetPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);

    Task<Fin<StripeRefundResponse>> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount = null,
        CancellationToken cancellationToken = default);

    Task<Fin<bool>> CancelPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);
}

public record StripePaymentIntentResponse
{
    public string PaymentIntentId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public string Status { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
}

public record StripeRefundResponse
{
    public string RefundId { get; init; } = null!;
    public string Status { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
}
