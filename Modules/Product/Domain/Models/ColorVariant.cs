using Shared.Domain.Enums;

namespace Product.Domain.Models;

public sealed record ColorVariant : IEntity<ColorVariantId>
{
    private ColorVariant()
    {
    }

    private ColorVariant(Color color)
    {
        Id = ColorVariantId.New;
        Color = color;
    }

    public ProductId ProductId { get; private init; }
    public Product Product { get; private init; }
    public Color Color { get; private set; }
    public ICollection<SizeVariant> SizeVariants { get; private set; } = [];
    public ICollection<Image> Images { get; private set; } = [];

    public ColorVariantId Id { get; }


    public bool Equals(ColorVariant? other)
    {
        return other is not null && Id == other.Id;
    }

    public static Fin<ColorVariant> Create(
        CreateColorVariantDto dto)
    {
        return Color.From(dto.Color).Map((co) => new ColorVariant(co));
    }

    public ColorVariant AddSizeVariants(params SizeVariant[] sizeVariants)
    {
        foreach (var sizeVariant in sizeVariants)
        {
            SizeVariants.Add(sizeVariant);
        }
        return this;
    }
    public ColorVariant AddImages(params Image[] images)
    {
        foreach (var image in images)
        {
            image.ColorVariantId = Id;
            Images.Add(image);
        }
        return this;
    }

    public ColorVariant UpdateImages(params UpdateImageDto[] dtos)
    {
        var (toDelete, toUpdate) = dtos.AsIterable().Partition(dto => dto.IsDeleted);
        foreach (var imageId in toDelete.Select(dto => dto.ImageId))
        {
            var imageToRemove = Images.FirstOrDefault(img => img.Id == imageId);
            if (imageToRemove != null)
            {
                Images.Remove(imageToRemove);
            }
        }

        foreach (var dto in toUpdate)
        {
            var image = Images.FirstOrDefault(img => img.Id == dto.ImageId);
            image?.Update(dto.AltText, dto.IsMain);
        }
        return this;
    }

    public Fin<ColorVariant> UpdateColor(string color, string category, string brand)
    {
        return Color.From(color).Map(c =>
         {
             foreach (var variant in SizeVariants)
             {
                 variant.ColorChangeHandle(c.Code.ToString(), category, brand);
             }
             Color = c;
             return this;
         });
    }

    public Fin<ColorVariant> UpdateStock(Guid sizeVariant, string size, int stock, StockLevel level, string brand, string category, string color)
    {
        return Optional(SizeVariants.FirstOrDefault(sv => sv.Id == sizeVariant)).ToFin()
             .BiBind<ColorVariant>(variant =>
             {
                 variant.UpdateStock(stock, level);
                 return this;
             }, _ =>
             {
                 var sv = SizeVariant.Create(sizeVariant, size, stock, level, brand, category, color);
                 SizeVariants.Add(sv);
                 return this;
             });

    }



    public override int GetHashCode()
    {
        return HashCode.Combine(Id.Value, Color);
    }

    //public ColorVariant CreateStock(Guid id, string size, int stock, StockLevel level, string brand, string category, string color)
    //{
    //    var sizeVariant = SizeVariant.Create(size, stock, level, brand, category, color);
    //    SizeVariants.Add(sizeVariant);
    //    return this;
    //}
}