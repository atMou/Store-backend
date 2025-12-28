namespace Order.Domain.Enums;
public enum OrderStatusCode
{
	Pending,
	PaymentFailed,
	Paid,
	Processing,
	Shipped,
	Delivered,
	Completed,
	Cancelled,
	Refunded,
	Returned,
	OnHold,
	Unknown
}