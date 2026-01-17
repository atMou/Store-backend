namespace Basket.Presentation.Requests;
public record GetCouponsByUserIdRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null);
