using Payment.Domain.ValueObjects;

using Shared.Application.Contracts.Payment.Results;

namespace Payment.Application.Contracts;

public static class Extensions
{
    public static PaymentResult ToResult(this Domain.Models.Payment payment)
    {

        return payment.PaymentStatus == PaymentStatus.Paid ? new PaymentResult
        {
            PaymentId = payment.Id.Value,
            Total = payment.Total,
            Tax = payment.Tax,
            Status = payment.PaymentStatus.ToString(),
            PaymentMethod = payment.PaymentMethod.ToString(),
            PaidAt = payment.PaidAt,
        } :
            payment.PaymentStatus == PaymentStatus.Refunded ? new PaymentResult
            {
                PaymentId = payment.Id.Value,
                Total = payment.Total,
                Tax = payment.Tax,
                Status = payment.PaymentStatus.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                RefundedAt = payment.RefundedAt,
            } :
            payment.PaymentStatus == PaymentStatus.Failed ? new PaymentResult
            {
                PaymentId = payment.Id.Value,
                Total = payment.Total,
                Tax = payment.Tax,
                Status = payment.PaymentStatus.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                FailedAt = payment.FailedAt,
            } : new PaymentResult
            {
                PaymentId = payment.Id.Value,
                Total = payment.Total,
                Tax = payment.Tax,
                Status = payment.PaymentStatus.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
            };


    }
}
