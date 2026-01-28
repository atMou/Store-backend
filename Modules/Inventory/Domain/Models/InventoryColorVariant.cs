namespace Inventory.Domain.Models;

public record InventoryColorVariant
{
    private InventoryColorVariant() { }
    private InventoryColorVariant(ColorVariantId colorVariantId, string color)
    {
        ColorVariantId = colorVariantId;
        Color = color;
    }
    public ColorVariantId ColorVariantId { get; private init; }
    public string Color { get; private init; }
    public ICollection<InventorySizeVariant> SizeVariants { get; private set; } = [];


    public static InventoryColorVariant Create(
        ColorVariantId colorVariantId,
        string color)
    {
        return new InventoryColorVariant(
            colorVariantId,
            color
        );
    }

    public InventoryColorVariant UpdateSizeVariant(InventorySizeVariant updatedSizeVariant)
    {
        SizeVariants = SizeVariants
            .Where(sv => sv.Id != updatedSizeVariant.Id)
            .Append(updatedSizeVariant)
            .ToList();
        return this;
    }

    public InventoryColorVariant AddSizeVariant(InventorySizeVariant sizeVariant)
    {
        SizeVariants = SizeVariants.Append(sizeVariant).ToList();
        return this;
    }

    public InventoryColorVariant RemoveSizeVariant(Guid sizeVariantId)
    {
        SizeVariants = SizeVariants.Where(sv => sv.Id != sizeVariantId).ToList();

        return this;
    }

    public Fin<InventoryColorVariant> UpdateSizeVariants(IEnumerable<UpdateInventorySizeDto> dtos)
    {

        var (toUpdate, toCreate) =
            dtos.AsIterable().Partition(dto => SizeVariants.Any(sv => sv.Id == dto.SizeVariantId));

        var updatedVariants = toUpdate.AsIterable().Traverse(dto =>

                SizeVariants.First(sv => sv.Id == dto.SizeVariantId)
                    .Update(dto.Warehouses, dto.Stock, dto.Low, dto.Mid, dto.High)
            ).Map(it => it.AsEnumerable());
        var createdVariants = toCreate.AsIterable().Traverse(dto =>
          InventorySizeVariant.Create(dto.Size, dto.Stock, dto.Low, dto.Mid, dto.High, dto.Warehouses)).Map(it => it.AsEnumerable());

        return (updatedVariants, createdVariants)
          .Apply((updated, created) =>
          {
              var updatedIds = updated.Select(sv => sv.Id).ToHashSet();
              var unchangedVariants = SizeVariants.Where(sv => !updatedIds.Contains(sv.Id));
              SizeVariants = unchangedVariants.Concat(updated).Concat(created).ToList();
              return this;
          }).As();

    }
}


