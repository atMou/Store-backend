using Product.Domain.Events;
using Shared.Application.Features.Inventory.Events;
using Shared.Domain.Enums;

using Brand = Shared.Domain.ValueObjects.Brand;

namespace Product.Domain.Models;

public class Product : Aggregate<ProductId>
{
    private Product() : base(ProductId.New()) { }

    private Product(
        Slug slug,
        Brand brand,
        Category category,
        Description description,
        Money price,
        Money? newPrice,
        Discount discount,
        IEnumerable<Attribute> sizeFitAttributes,
        IEnumerable<Attribute> detailsAttributes,
        IEnumerable<MaterialDetail> materialDetails,
        ProductType productType
        ) : this()
    {
        Slug = slug;
        Brand = brand;
        Category = category;
        Description = description;
        Price = price;
        NewPrice = newPrice;
        Discount = discount;
        Status = Status.New;
        SizeFitAttributes = sizeFitAttributes.ToList();
        DetailsAttributes = detailsAttributes.ToList();
        MaterialDetails = materialDetails.ToList();
        ProductType = productType;
    }

    public Slug Slug { get; private set; }
    public Brand Brand { get; private set; }
    public Category Category { get; private set; }
    public ProductType ProductType { get; private set; }
    public Discount Discount { get; private set; }
    public Money Price { get; private set; }
    public Money? NewPrice { get; private set; }
    public Description Description { get; private set; }
    public Status Status { get; private set; }
    public int TotalReviews { get; private set; }

    public int TotalSales { get; private set; }
    public double AverageRating { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool HasInventory { get; private set; }
    public ProductId? ParentProductId { get; private init; }
    public Product? ParentProduct { get; private init; }
    public ICollection<Product> Alternatives { get; private set; } = [];
    public ICollection<MaterialDetail> MaterialDetails { get; private set; } = [];
    public ICollection<Attribute> DetailsAttributes { get; private set; } = [];
    public ICollection<Attribute> SizeFitAttributes { get; private set; } = [];
    public ICollection<ColorVariant> ColorVariants { get; private set; } = [];
    public ICollection<Review> Reviews { get; private set; } = [];
    public ICollection<Image> Images { get; private set; } = [];

    [NotMapped]
    public IEnumerable<Color> Colors => ColorVariants
        .Select(v => v.Color)
        .DistinctBy(c => c.Code);

    [NotMapped]
    public IEnumerable<Size> Sizes => ColorVariants
        .SelectMany(v => v.SizeVariants)
        .Select(sv => sv.Size)
        .DistinctBy(s => s.Code)
        .OrderBy(size => size.Order);


    [NotMapped]
    public Rating Rating =>
        Rating.From(AverageRating).IfFail(_ => Rating.None);

    public virtual bool Equals(Product? other)
    {
        return other is not null && Id == other.Id;
    }

    public static Fin<Product> Create(CreateProductDto dto)
    {


        var sizeFitAttribute = dto.SizeFitAttributes
            .AsIterable()
            .Traverse(a => Attribute.From(a.Name, a.Description))
            .Map(it => it.ToList()).As();

        var detailsAttribute = dto.DetailsAttributes.AsIterable()
            .Traverse(a => Attribute.From(a.Name, a.Description))
            .Map(it => it.ToList()).As();

        var materialDetails = dto.MaterialDetails.AsIterable()
            .Traverse(md => MaterialDetail.From(md.Detail, md.Percentage, md.Material))
            .Map(it => it.ToList()).As();

        var productType = ProductType.From(dto.Type, dto.SubType);

        return (
            Slug.From(dto.Slug),
            Brand.From(dto.Brand),
            Category.From(dto.Category, dto.SubCategory)(productType),
            Description.From(10, 1000, dto.Description),
            Discount.From(dto.Price, dto.NewPrice),
            productType,
            sizeFitAttribute,
            detailsAttribute,
            materialDetails
        ).Apply((slug, brand, category, description, discount, type, sfAttr, dtAttr, md) => new Product(
            slug,
            brand,
            category,
            description,
            dto.Price,
            dto.NewPrice,
            discount,
            sfAttr,
            dtAttr,
            md,
            type
        )).As();
    }

    public Product MarkAsDeleted()
    {
        IsDeleted = true;
        return this;
    }

    public Product MarkAsRestored()
    {
        IsDeleted = false;
        return this;
    }

    public Fin<Product> UpdateStock(Guid colorVariantId, Guid sizeVariantId, string size, int stock, StockLevel level)
    {
        var colorVariant = ColorVariants.FirstOrDefault(v => v.Id.Value == colorVariantId);

        if (colorVariant is null)
            return FinFail<Product>(NotFoundError.New($"Color variant with ID '{colorVariantId}' not found"));

        return colorVariant.UpdateStock(sizeVariantId, size, stock, level, Brand.Code.ToString(), Category.ToString(), colorVariant.Color.Code.ToString())
            .Map(_ => this);
    }

    // internal use only
    public Fin<Product> UpdateColorVariants(params InventoryColorVariantDto[] colorVariants)
    {

        return ColorVariants.AsIterable().Fold(FinSucc(this), (current, cv) =>
         {
             var dto = colorVariants.FirstOrDefault(d => d.ColorVariantId == cv.Id.Value);
             if (dto is null)
                 return current;
             return dto.SizeVariants.AsIterable().Fold(current, (curr, usv) =>
                 curr.Bind(p => p.UpdateStock(dto.ColorVariantId, usv.SizeVariantId, usv.Size, usv.Stock, usv.Level))
             );
         });

    }

    private Fin<Product> UpdateSlug(string slug)
    {
        return Slug.From(slug).Map(s =>
        {
            Slug = s;
            return this;
        });
    }
    private Fin<Product> UpdateBrand(string brand)
    {
        return Brand.From(brand).Map(b =>
        {
            foreach (var colorVariant in ColorVariants)
            {
                foreach (var sizeVariant in colorVariant.SizeVariants)
                {
                    sizeVariant.BrandChangeHandle(b.Code.ToString(), Category.ToString(), colorVariant.Color.Code.ToString());
                }
            }
            Brand = b;

            return this;
        });
    }

    private Fin<Product> UpdateCategory(string main, string sub, string productType, string productSub)
    {
        return Category.From(main, sub)(ProductType.From(productType, productSub)).Map(c =>
        {
            foreach (var colorVariant in ColorVariants)
            {
                foreach (var sizeVariant in colorVariant.SizeVariants)
                {
                    sizeVariant.CategoryChangeHandle(Category.ToString(), Brand.Code.ToString(), colorVariant.Color.Code.ToString());
                }
            }
            Category = c;
            return this;
        });
    }

    public Fin<Product> UpdateColorVariants(params Contracts.UpdateColorVariantDto[] colorVariants)
    {
        var (toUpdate, toCreate) = colorVariants.AsIterable().Partition(dto => ColorVariants.Any(cv => dto.ColorVariantId == cv.Id));

        var updated = toUpdate.Traverse(dto => ColorVariants.First(cv => cv.Id == dto.ColorVariantId).UpdateColor(dto.Color, Category.ToString(), Brand.Name));
        var created = toCreate.Traverse(dto => ColorVariant.Create(new CreateColorVariantDto() { Color = dto.Color }));

        return (updated, created)
            .Apply((u, c) =>
            {
                ColorVariants = [.. u, .. c];
                return this;
            }).As();


    }


    public Product AddImages(params Image[] images)
    {
        foreach (var image in images)
        {
            image.ProductId = Id;
            Images.Add(image);
        }
        return this;
    }

    private Product UpdateStatus(bool isFeatured, bool isTrending, bool isBestSeller, bool isNew)
    {
        Status = Status.Update(isFeatured, isTrending, isBestSeller, isNew);
        return this;
    }

    public Product AddImages(ColorVariantId colorVariantId, params Image[] images)
    {
        var variant = ColorVariants.FirstOrDefault(v => v.Id == colorVariantId);
        if (variant is null) return this;

        variant.AddImages(images);
        return this;
    }

    private Product UpdateImages(params UpdateImageDto[] dtos)
    {
        // Handle deletions
        var imagesToDelete = dtos.Where(dto => dto.IsDeleted).Select(dto => dto.ImageId).ToList();
        foreach (var imageId in imagesToDelete)
        {
            var imageToRemove = Images.FirstOrDefault(img => img.Id == imageId);
            if (imageToRemove != null)
            {
                Images.Remove(imageToRemove);
            }
        }

        var _dtos = dtos.Where(dto => !dto.IsDeleted).ToList();
        foreach (var dto in _dtos)
        {
            var image = Images.FirstOrDefault(img => img.Id == dto.ImageId);
            if (image != null)
            {
                image.Update(dto.AltText, dto.IsMain);
            }
        }

        return this;
    }

    private Fin<Product> UpdateMaterialDetails(params UpdateMaterialDetailDto[] dtos)
    {
        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<MaterialDetail>>();

        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {
            var materialDetail = MaterialDetails.FirstOrDefault(md => md.Detail == dto.Detail);
            return materialDetail is not null
                ? current.Add(materialDetail.Update(dto.Detail, dto.Percentage, dto.Material))
                : current.Add(MaterialDetail.From(dto.Detail, dto.Percentage, dto.Material));
        });

        return result.Traverse(identity)
            .Map(mds =>
            {
                MaterialDetails = mds.ToList();
                return this;
            }).As();
    }

    private Fin<Product> UpdateDetailsAttributes(params UpdateAttributeDto[] dtos)
    {
        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<Attribute>>();

        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {
            var attribute = DetailsAttributes.FirstOrDefault(attr => attr.Name == dto.Name);
            return attribute is not null
                ? current.Add(attribute.Update(dto.Name, dto.Description))
                : current.Add(Attribute.From(dto.Name, dto.Description));
        });

        return result.Traverse(identity)
            .Map(attrs =>
            {
                DetailsAttributes = attrs.ToList();
                return this;
            }).As();
    }

    private Fin<Product> UpdateSizeFitAttributes(params UpdateAttributeDto[] dtos)
    {
        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<Attribute>>();

        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {
            var attribute = SizeFitAttributes.FirstOrDefault(attr => attr.Name == dto.Name);
            return attribute is not null
                ? current.Add(attribute.Update(dto.Name, dto.Description))
                : current.Add(Attribute.From(dto.Name, dto.Description));
        });

        return result.Traverse(identity)
            .Map(attrs =>
            {
                SizeFitAttributes = attrs.ToList();
                return this;
            }).As();
    }

    public Product AddColorVariants(IEnumerable<ColorVariant> variants)
    {
        foreach (var variant in variants)
        {
            ColorVariants.Add(variant);
        }
        return this;
    }

    public Product DeleteImages(IEnumerable<ImageId> ids)
    {
        Images = Images.Where(pi => !ids.Contains(pi.Id)).ToList();
        return this;
    }


    private Fin<Product> UpdateProductType(string type, string sub)
    {
        return ProductType.From(type, sub)
            .Map(pt =>
            {
                ProductType = pt;
                return this;
            });
    }

    private Fin<Product> UpdateDescription(string description)
    {
        return Description.From(10, 1000, description).Map(d =>
        {
            Description = d;
            return this;
        });
    }


    private Product UpdateTotalSales(int soldItems = 1)
    {
        var newTotal = TotalSales + soldItems;
        TotalSales = newTotal;
        return this;
    }

    private Product UpdatePrice(decimal price)
    {
        Price = Money.FromDecimal(price);
        return this;
    }

    private Product UpdateNewPrice(decimal price)
    {
        NewPrice = Money.FromDecimal(price);
        return this;
    }

    public Product AddAlternatives(params Product[] products)
    {
        var alternatives = new List<Product>([.. products, .. Alternatives])
            .DistinctBy(p => p.Id.Value);
        Alternatives = alternatives.ToList();
        return this;
    }

    public Product RemoveAlternatives(params Product[] products)
    {
        Alternatives = Alternatives.Where(v => !products.Contains(v)).ToList();
        return this;
    }

    public Product AddReview(Review review)
    {
        if (review.Rating <= Rating.Fair)
            AddDomainEvent(new ProductReviewAddedEvent(Id, review.UserId, review.Id, review.Rating));

        Reviews = Reviews.Append(review).ToList();
        TotalReviews = Reviews.Count;
        AverageRating = CalculateRating(review.Rating, Reviews.Select(r => r.Rating));
        return this;
    }

    public Product UpdateReview(Review review)
    {
        var oldReview = Reviews.FirstOrDefault(r => r.Id == review.Id);
        if (oldReview is null) return this;

        var updatedReviews = Reviews
            .Where(r => r.Id != review.Id)
            .Append(review)
            .ToList();

        Reviews = updatedReviews;
        AverageRating = updatedReviews.Any()
            ? updatedReviews.Average(r => r.Rating.Value)
            : 0;

        return this;
    }

    public Product DeleteReview(Review review)
    {
        Reviews = Reviews.Where(r => r.Id != review.Id).ToList();
        AverageRating = Reviews.Count() > 1
            ? Reviews.Average(r => r.Rating.Value)
            : 0;
        TotalReviews = Reviews.Count;
        return this;
    }

    private double CalculateRating(Rating rating, IEnumerable<Rating> ratings)
    {
        var rts = ratings.ToArray();
        if (!rts.Any()) return rating.Value;

        var totalRating = rts.Sum(r => r.Value) + rating.Value;
        var averageRating = totalRating / (rts.Length + 1);
        return averageRating;
    }

    public Fin<Product> Update(UpdateProductDto dto, IEnumerable<Product> alternatives)
    {
        var fin = FinSucc(this);

        if (Slug.Value != dto.Slug)
            fin = fin.Bind(p => p.UpdateSlug(dto.Slug));

        if (Brand.Code.ToString() != dto.Brand)
            fin = fin.Bind(p => p.UpdateBrand(dto.Brand));

        if (dto.Category != Category.Main || dto.SubCategory != Category.Sub)
            fin = fin.Bind(p => p.UpdateCategory(dto.Category, dto.SubCategory, dto.Type, dto.SubType));

        if (dto.Type != ProductType.Type || dto.SubType != ProductType.SubType)
            fin = fin.Bind(p => p.UpdateProductType(dto.Type, dto.SubType));

        if (Description.Value != dto.Description)
            fin = fin.Bind(p => p.UpdateDescription(dto.Description));

        if (dto.Variants.Any())
            fin = fin.Bind(p => p.UpdateColorVariants([.. dto.Variants]));

        if (Price.Value != dto.Price)
            fin = fin.Map(p => p.UpdatePrice(dto.Price));

        if (dto.NewPrice is { } np && dto.NewPrice != NewPrice?.Value)
            fin = fin.Map(p => p.UpdateNewPrice(np));

        if (dto.AlternativesIds.Any())
            fin = fin.Map(p => p.AddAlternatives([.. alternatives]));

        if (dto.MaterialDetails.Any())
            fin = fin.Bind(p => p.UpdateMaterialDetails([.. dto.MaterialDetails]));

        if (dto.DetailsAttributes.Any())
            fin = fin.Bind(p => p.UpdateDetailsAttributes([.. dto.DetailsAttributes]));

        if (dto.SizeFitAttributes.Any())
            fin = fin.Bind(p => p.UpdateSizeFitAttributes([.. dto.SizeFitAttributes]));

        if (dto.IsFeatured != Status.IsFeatured ||
            dto.IsNew != Status.IsNew ||
            dto.IsBestSeller != Status.IsBestSeller ||
            dto.IsTrending != Status.IsTrending)
        {
            fin = fin.Map(p => p.UpdateStatus(dto.IsFeatured, dto.IsTrending, dto.IsBestSeller, dto.IsNew));
        }

        if (dto.ImageDtos.Any())
            fin = fin.Map(p => p.UpdateImages([.. dto.ImageDtos]));

        return fin;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public Fin<Product> UpdateVariantImages(params Contracts.UpdateColorVariantDto[] variantDtos)
    {
        foreach (var dto in variantDtos.Where(d => d.ImageDtos?.Any() == true))
        {
            var variant = ColorVariants.FirstOrDefault(v => v.Id == dto.ColorVariantId);
            variant?.UpdateImages([.. dto.ImageDtos]);
        }
        return this;
    }


}

