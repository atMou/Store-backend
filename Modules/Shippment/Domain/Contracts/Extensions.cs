using Shared.Application.Contracts.Shipment.Results;

namespace Shipment.Domain.Contracts;

public static class Extensions
{
    public static ShipmentResult ToResult(this Models.Shipment shipment)
    {
        return new ShipmentResult
        {
            Id = shipment.Id.Value,
            OrderId = shipment.OrderId.Value,
            Status = shipment.ShippingStatus.Name,
            ShippingAddress = new AddressResult
            {
                Street = shipment.ShippingAddress.Street,
                City = shipment.ShippingAddress.City,
                PostalCode = shipment.ShippingAddress.PostalCode,
                HouseNumber = shipment.ShippingAddress.HouseNumber,
                ExtraDetails = shipment.ShippingAddress.ExtraDetails
            },
            TrackingCode = shipment.TrackingCode,
            ShippedAt = shipment.ShippedAt,
            DeliveredAt = shipment.DeliveredAt,
            CreatedAt = shipment.CreatedAt
        };
    }
}
