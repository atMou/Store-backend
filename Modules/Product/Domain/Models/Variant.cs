namespace Product.Domain.Models;

public sealed record Variant : IEntity<VariantId>
{
    private Variant()
    {
    }

    private Variant(Color color, IEnumerable<SizeVariant> sizeVariants)
    {
        Id = VariantId.New;
        Color = color;
        SizeVariants = sizeVariants.ToList();
    }

    public ProductId ProductId { get; private init; } = null!;
    public Product Product { get; private init; }
    public Sku Sku { get; private set; } = null!;
    public Color Color { get; private set; }
    public ICollection<SizeVariant> SizeVariants { get; private set; } = [];
    public ICollection<Image> Images { get; private set; } = [];

    public VariantId Id { get; }


    public bool Equals(Variant? other)
    {
        return other is not null &&
               Color.Equals(other.Color) &&
               Sku.Equals(other.Sku);
    }

    public static Fin<Variant> Create(
        CreateVariantDto dto,
        string brand,
        string category)
    {
        var sizes = dto.SizeVariants.AsIterable()
            .Traverse(sizeVariantDto => Size.From(sizeVariantDto.Size).Map(SizeVariant.Create)).As();
        return (Color.From(dto.Color), sizes).Apply((co, si) =>
        {
            var sku = Sku.From(
                category,
                co.Code.ToString(),
                brand
            );
            return new Variant(co, si)
            {
                Sku = sku
            };
        }).As();
    }

    public Variant AddImages(params Image[] images)
    {
        Images = [.. Images, .. images];
        return this;
    }

    private Variant UpdateImages(params UpdateImageDto[] dtos)
    {
        var _dtos = dtos.Where(dto => !dto.IsDeleted).ToList();
        var imagesToUpdate = Images.Where(img => _dtos.Any(dto => dto.ImageId == img.Id)).ToList();

        Images = imagesToUpdate.Select(UpdateFunc).ToList();
        return this;

        Image UpdateFunc(Image img)
        {
            return Optional(_dtos.FirstOrDefault(dto => dto.ImageId == img.Id))
                .Match(
                    dto => img.Update(dto.AltText, dto.IsMain),
                    () => img
                );
        }
    }

    private Fin<Variant> UpdateSize(string size, Brand brand, Category category)
    {
        return Size.From(size).Map(sz =>
        {
            Sku = Sku.From(
                category.ToString(),
                Color.Code.ToString(),
                brand.Code.ToString()
            );
            return this;
        });
    }

    public Fin<Variant> UpdateColor(string color, Brand brand, Category category)
    {
        return Color.From(color).Map(c =>
        {
            Color = c;
            Sku = Sku.From(
                category.ToString(),
                c.Code.ToString(),
                brand.Code.ToString()
            );
            return this;
        });
    }

    public Variant UpdateSkuForBrand(Brand brand, Category category)
    {
        Sku = Sku.From(
            category.ToString(),
            Color.Code.ToString(),
            brand.Code.ToString()
        );
        return this;
    }

    public Variant UpdateSkuForCategory(Brand brand, Category category)
    {
        Sku = Sku.From(
            category.ToString(),
            Color.Code.ToString(),
            brand.Code.ToString()
        );
        return this;
    }

    public Fin<Variant> Update(UpdateVariantDto dto, Category category, Brand brand)
    {
        var fin = FinSucc(this);

        if (dto.Color != Color.Name)
            fin = fin.Bind(v => v.UpdateColor(dto.Color, brand, category));

        fin = fin.Map(v => v.UpdateImages([.. dto.ImageDtos]));

        return fin;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Color, Sku);
    }
}