namespace Payment.Presentation.Requests;

public record ProcessStripeRefundRequest
{
    public decimal? RefundAmount { get; init; }
}
