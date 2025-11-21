namespace Payment.Domain.Enums;
public enum PaymentStatusCode
{
    Pending,
    Failed,
    Authorized,
    Paid,
    Refunded,
    Voided,
    Unknown
}
