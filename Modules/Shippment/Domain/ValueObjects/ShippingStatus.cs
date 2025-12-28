using Shipment.Domain.Enums;


namespace Shipment.Domain.ValueObjects;

public record ShippingStatus
{
	private static readonly List<ShippingStatus> _all = new();
	private List<ShippingStatus> AllowedStatusChangeTo { get; set; } = new();

	private ShippingStatus(ShippingStatusCode code, string name, string description)
	{
		Code = code;
		Name = name;
		Description = description;
		_all.Add(this);
	}

	public ShippingStatusCode Code { get; private set; }
	public string Name { get; private set; } = null!;
	public string Description { get; private set; }

	public static IReadOnlyList<ShippingStatus> All => _all;

	public static readonly ShippingStatus Pending = new(ShippingStatusCode.Pending, nameof(Pending), "Shipment pending");
	public static readonly ShippingStatus Shipped = new(ShippingStatusCode.Shipped, nameof(Shipped), "Shipment dispatched");
	public static readonly ShippingStatus Delivered = new(ShippingStatusCode.Delivered, nameof(Delivered), "Shipment delivered to customer");
	public static readonly ShippingStatus OnHold = new(ShippingStatusCode.OnHold, nameof(OnHold), "Shipment temporarily on hold");
	public static readonly ShippingStatus Cancelled = new(ShippingStatusCode.Cancelled, nameof(Cancelled), "Shipment cancelled");
	public static readonly ShippingStatus Unknown = new(ShippingStatusCode.Unknown, nameof(Unknown), "Unknown shipment status");

	static ShippingStatus()
	{
		Pending.AllowedStatusChangeTo = [Shipped, OnHold, Cancelled];
		Shipped.AllowedStatusChangeTo = [Delivered, OnHold];
		Delivered.AllowedStatusChangeTo = [];
		OnHold.AllowedStatusChangeTo = [Pending, Shipped, Cancelled];
		Cancelled.AllowedStatusChangeTo = [];
		Unknown.AllowedStatusChangeTo = [];
	}

	public static ShippingStatus From(string name) =>
		All.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
		?? Unknown;


	public Fin<Unit> CanTransitionTo(ShippingStatus target) =>
		AllowedStatusChangeTo.Contains(target)
			? FinSucc(unit)
			: FinFail<Unit>((Error)$"Cannot transition from '{Name}' to '{target.Name}'.");
}
