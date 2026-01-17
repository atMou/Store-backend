namespace Payment.Domain.Enums;
public enum PaymentStatusCode
{
    Pending,
    Processing,
    Failed,
    Authorized,
    Paid,
    Refunded,
    Voided,
    Cancelled,
    Unknown
}
