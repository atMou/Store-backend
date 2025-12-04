using Inventory.Domain.ValueObjects;

using Shared.Domain.Enums;

using Attribute = Product.Domain.ValueObjects.Attribute;
// ReSharper disable InconsistentNaming

namespace Product.Domain.Models;

public record Variant : Entity<VariantId>
{
    private Variant() : base(VariantId.New)
    {
    }

    public Variant(Color color, Size size, Stock stock, IEnumerable<Attribute> attributes) : this()
    {
        Color = color;
        Size = size;
        Stock = stock;
        Attributes = attributes;
    }

    public ProductId ProductId { get; private init; }
    public Sku Sku { get; private init; }
    public Color Color { get; private init; }
    public Size Size { get; private init; }
    public IEnumerable<ProductImage> Images { get; private init; } = [];
    public IEnumerable<Attribute> Attributes { get; init; }
    public Stock Stock { get; private init; }
    public StockLevel StockLevel =>
        Stock.Value switch
        {
            <= 0 => StockLevel.OutOfStock,
            var s when s <= Stock.Low => StockLevel.LowStock,
            var s when s <= Stock.Mid => StockLevel.MediumStock,
            _ => StockLevel.HighStock
        };


    public static Fin<Variant> Create(
     CreateVariantDto dto,
        string brand,
        string category)
    {
        var _attributes = dto.Attributes.AsIterable().Traverse(a =>
            Attribute.Create(a.Name, a.Description)).Map(a => a.AsEnumerable()).As();

        var stock = Stock.From((dto.Stock, dto.StockLow, dto.StockMid, dto.StockHigh));

        return (Color.FromCode(dto.Color), Size.FromCode(dto.Size), _attributes,
                stock)
            .Apply((co, si, ats, st) =>

            {
                var sku = Sku.From(
                    category,
                    co.Name,
                    si.Name,
                    brand
                );
                return new Variant(co, si, st, ats)
                {
                    Sku = sku
                };
            }).As();
    }


    public Variant AddImages(params ImageResult[] imageResult)
    {
        return this with
        {
            Images =
            imageResult.Select(result => ProductImage.FromUnsafe(result.Url, result.AltText, result.IsMain))
        };
    }

    public Fin<Variant> UpdateImages(params UpdateVariantImageDto[] dtos)
    {

        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<ProductImage>>();
        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {

            var existing = Images.FirstOrDefault(img => img.Id == dto.ProductImageId);
            return existing is not null
                ? current.Add(existing.Update(dto.Url, dto.AltText, dto.IsMain))
                : current.Add(ProductImage.From(dto.Url, dto.AltText, dto.IsMain));
        });

        return result.Traverse(identity)
            .Map(imgs => this with { Images = imgs.AsEnumerable() }).As();
    }


    public Fin<Variant> Update(UpdateVariantDto dto, Category category, Brand brand)
    {
        var validationSeq = Seq<Fin<Variant>>();
        if (dto.Color != Color.Name)
        {
            validationSeq = validationSeq.Add(UpdateColor(dto.Color, brand, category));
        }
        if (dto.Size != Size.Name)
        {
            validationSeq = validationSeq.Add(UpdateSize(dto.Size, brand, category));
        }

        if (dto.Stock != Stock.Value)
        {
            validationSeq = validationSeq.Add(UpdateStock(dto.Stock));
        }

        if (dto.Attributes.Any())
        {
            validationSeq = validationSeq.Add(UpdateAttributes(dto.Attributes));
        }

        if (dto.ImageDtos.Any())
        {
            validationSeq = validationSeq.Add(UpdateImages([.. dto.ImageDtos]).Map(_ => this));
        }
        return validationSeq.Traverse(identity)
            .Map(seq => seq.Last.IfNone(() => this)).As();
    }

    private Fin<Variant> UpdateAttributes(IEnumerable<UpdateAttributeDto> attributes)
    {
        var validationSeq = Seq<Fin<Attribute>>();
        foreach (var dto in attributes)
        {
            var existing = Attributes.FirstOrDefault(attribute => attribute.Name == dto.Name);
            if (existing is { })
            {
                validationSeq = validationSeq.Add(existing.Update(dto.Description));
            }
            else
            {
                validationSeq = validationSeq.Add(Attribute.Create(dto.Name, dto.Description));
            }
        }
        return validationSeq.Traverse(identity).Map(ats => this with { Attributes = ats.AsEnumerable() }).As();

    }

    public Fin<Variant> UpdateStock(int stock, int? low = null, int? mid = null, int? high = null)
    {
        var _stock = Stock.From((stock, low ?? Stock.Low, mid ?? Stock.Mid, high ?? Stock.High));
        return _stock.Map(s => this with { Stock = s });
    }


    public Fin<Variant> UpdateSize(string size, Brand brand, Category category)
    {
        return Size.FromCode(size).Map(s => this with
        {
            Size = s,
            Sku = Sku.From(
                category.Code.ToString(),
                Color.Code.ToString(),
                s.Code.ToString(),
                brand.Code.ToString()
            )
        });
    }

    public Fin<Variant> UpdateColor(string color, Brand brand, Category category)
    {
        return Color.FromCode(color).Map(c => this with
        {
            Color = c,
            Sku = Sku.From(
                category.Code.ToString(),
                c.Code.ToString(),
                Size.Code.ToString(),
                brand.Code.ToString()
            )
        });
    }

    public Variant UpdateSkuForBrand(Brand brand, Category category)
    {
        return this with
        {
            Sku = Sku.From(
                category.Code.ToString(),
                Color.Code.ToString(),
                Size.Code.ToString(),
                brand.Code.ToString())
        };
    }

    public Variant UpdateSkuForCategory(Brand brand, Category category)
    {
        return this with
        {
            Sku = Sku.From(
                category.Code.ToString(),
                Color.Code.ToString(),
                Size.Code.ToString(),
                brand.Code.ToString())
        };
    }
}