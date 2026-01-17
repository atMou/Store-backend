namespace Payment.Domain.Enums;
public enum PaymentMethodCode
{
    Stripe,
    CreditCard,
    DebitCard,
    PayPal,
    BankTransfer,
    CashOnDelivery,
    Unknown
}