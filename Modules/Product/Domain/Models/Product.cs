
namespace Product.Domain.Models;
//add-migration init -OutputDir Persistence/Migrations -context ProductDBContext -Project Product -StartUpProject Api
//add-migration init -OutputDir Persistence/Migrations -context BasketDBContext -Project Basket -StartUpProject Api
//add-migration init -OutputDir Persistence/Migrations -context IdentityDBContext -Project Identity -StartUpProject Api
//add-migration init -OutputDir Persistence/Migrations -context InventoryDBContext -Project Inventory -StartUpProject Api
//add-migration init -OutputDir Persistence/Migrations -context OrderDbContext -Project Order -StartUpProject Api
//update-database -context ProductDBContext
//update-database -context BasketDBContext 
//update-database -context IdentityDBContext
//update-database -context OrderDbContext
//remove-migration -context ProductDBContext 
//remove-migration -context BasketDBContext
public record Product : Aggregate<ProductId>, ISoftDeletable
{
    private Product() : base(ProductId.New())
    {
    }

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
        ProductType productType) : this()
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

    public int TotalSales { get; private init; }
    public double AverageRating { get; private set; }
    public bool IsDeleted { get; private set; }
    public ProductId? ParentProductId { get; private init; }
    public Product? ParentProduct { get; private init; }
    public ICollection<Product> Alternatives { get; private set; } = [];
    public ICollection<MaterialDetail> MaterialDetails { get; private set; } = [];
    public ICollection<Attribute> DetailsAttributes { get; private set; } = [];
    public ICollection<Attribute> SizeFitAttributes { get; private set; } = [];
    public ICollection<Variant> Variants { get; private set; } = [];
    public ICollection<Review> Reviews { get; private set; } = [];
    public ICollection<Image> Images { get; private set; } = [];

    [NotMapped]
    public IEnumerable<Color> Colors => Variants
        .Select(v => v.Color)
        .DistinctBy(c => c.Code);

    [NotMapped]
    public IEnumerable<Size> Sizes => Variants
        .Select(v => v.Size)
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

    private Fin<Product> UpdateSlug(string slug)
    {
        return Slug.From(slug).Map(s =>
        {
            Slug = s;
            return this;
        });
    }

    public Product AddImages(params Image[] images)
    {
        Images = [.. Images, .. images];
        return this;
    }

    private Product UpdateStatus(bool isFeatured, bool isTrending, bool isBestSeller, bool isNew)
    {
        Status = Status.Update(isFeatured, isTrending, isBestSeller, isNew);
        return this;
    }

    public Product AddImages(VariantId variantId, params Image[] images)
    {
        var variant = Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null) return this;

        var updatedVariant = variant.AddImages(images);
        Variants = Variants
            .Where(v => v.Id != variantId)
            .Append(updatedVariant)
            .ToList();

        return this;
    }

    private Product UpdateImages(params UpdateImageDto[] dtos)
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

    public Fin<Product> UpdateVariants(params UpdateVariantDto[] dtos)
    {
        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<Variant>>();

        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {
            var variant = Variants.FirstOrDefault(v => v.Id == dto.VariantId);
            return variant is not null
                ? current.Add(variant.Update(dto, Category, Brand))
                : current.Add(Variant.Create(
                    new CreateVariantDto
                    {
                        Color = dto.Color,
                        SizeVariants = dto.SizeVariants,
                    }, Brand.Name, Category.ToString()));
        });

        return result.Traverse(identity)
            .Map(vs =>
            {
                Variants = vs.ToList();
                return this;
            }).As();
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

    public Product AddVariants(IEnumerable<Variant> variants)
    {
        Variants = variants.ToList();
        return this;
    }

    public Product DeleteImages(IEnumerable<ImageId> ids)
    {
        Images = Images.Where(pi => !ids.Contains(pi.Id)).ToList();
        return this;
    }
    public Product UpdateVariantsStock(IEnumerable<Variant> variants)
    {
        Variants = variants.ToList();
        return this;
    }
    private Fin<Product> UpdateBrand(string brand)
    {
        return Brand.From(brand).Map(b =>
        {
            Brand = b;
            Variants = Variants.Select(v => v.UpdateSkuForBrand(b, Category)).ToList();
            return this;
        });
    }

    private Fin<Product> UpdateCategory(string category, string sub)
    {
        var p = Category.From(category, sub)(ProductType)
            .Map(c =>
            {
                Category = c;
                Variants = Variants.Select(v => v.UpdateSkuForCategory(Brand, c)).ToList();
                return this;
            });
        return p;
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
        return this with { TotalSales = newTotal };
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

    private Product AddAlternatives(params Product[] products)
    {
        if (!products.Any()) return this;
        var newVariants = new List<Product>([.. products, .. Alternatives])
            .DistinctBy(p => p.Id.Value);
        Alternatives = newVariants.ToList();
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
            fin = fin.Bind(p => p.UpdateCategory(dto.Category, dto.SubCategory));

        if (dto.Type != ProductType.Type || dto.SubType != ProductType.SubType)
            fin = fin.Bind(p => p.UpdateProductType(dto.Type, dto.SubType));

        if (Description.Value != dto.Description)
            fin = fin.Bind(p => p.UpdateDescription(dto.Description));

        fin = fin.Bind(p => p.UpdateVariants([.. dto.Variants]));

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

        fin = fin.Map(p => p.UpdateImages([.. dto.ImageDtos]));

        return fin;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }



}

