using Shared.Domain.Enums;

namespace Product.Domain.Models;

//add-migration init -OutputDir Persistence/Data/Migrations -context ProductDBContext -Project Product -StartUpProject Api
//add-migration init -OutputDir Persistence/Data/Migrations -context BasketDBContext -Project Basket -StartUpProject Api
//add-migration init -OutputDir Persistence/Data/Migrations -context IdentityDBContext -Project Identity -StartUpProject Api
//add-migration init -OutputDir Persistence/Data/Migrations -context OrderDbContext -Project Order -StartUpProject Api
public record Product : Aggregate<ProductId>
{
    private Product() : base(ProductId.New())
    {
    }

    private Product(
        Slug slug,
        Sku sku,
        Brand brand,
        Size size,
        Color color,
        Category category,
        Description description,
        Money price,
        Money? newPrice,
        Discount discount
    ) : base(ProductId.New())
    {
        Slug = slug;
        Sku = sku;
        Brand = brand;
        Size = size;
        Color = color;
        Category = category;
        Description = description;
        Price = price;
        NewPrice = newPrice;
        Discount = discount;
        Status = Status.New;
    }

    public Sku Sku { get; private init; }
    public Slug Slug { get; private init; }
    public Brand Brand { get; private init; }
    public Size Size { get; private init; }
    public Color Color { get; private init; }
    public Category Category { get; private init; }
    public Discount Discount { get; private init; }
    public Money Price { get; private init; }
    public Description Description { get; private init; }
    public Status Status { get; }
    public int Stock { get; private init; }
    public StockLevel StockLevel { get; private set; }

    public int TotalReviews { get; private init; }
    public int TotalSales { get; private init; }
    public double AverageRating { get; private init; }
    public bool IsDeleted { get; private init; }
    public Money? NewPrice { get; private init; }
    public ProductId? ParentProductId { get; private init; }
    public Product? ParentProduct { get; private init; }
    public List<Product> Variants { get; private init; } = [];
    public List<Review> Reviews { get; private init; } = [];
    public List<ProductImage> ProductImages { get; private set; } = [];


    [NotMapped]
    public Rating Rating =>
        Rating.From(AverageRating)
            .Match(rating => rating, _ => Rating.None);

    public static Fin<Product> Create(CreateProductDto dto)
    {

        return (
                Slug.From(dto.Slug),
                Brand.FromCode(dto.Brand),
                Size.FromCode(dto.Size),
                Color.FromCode(dto.Color),
                Category.FromCode(dto.Category),
                Description.From(dto.Description),
                Discount.From(dto.Price, dto.NewPrice)

        ).Apply((slug, brand, size, color, category, description, discount) =>
        {
            var sku = Sku.From(
                category.Code.ToString(),
                color.Code.ToString(),
                size.Code.ToString(),
                brand.Code.ToString()
            );
            return new Product(
                slug,
                sku,
                brand,
                size,
                color,
                category,
                description,
                dto.Price,
                dto.NewPrice,
                discount

            );
        }).As()
        .Map(p =>
        {
            p.AddDomainEvent(new ProductCreatedDomainEvent(p));
            return p;
        });
    }

    public Product UpdateStock(int stock, StockLevel stockLevel)
    {
        return this with { Stock = stock, StockLevel = stockLevel };
    }

    public Product MarkAsDeleted()
    {
        return this with { IsDeleted = true };
    }
    private Fin<Product> UpdateSlug(string slug)
    {
        return Slug.From(slug).Map(s => this with { Slug = s });
    }

    public Fin<Product> AddImages(params ImageResult[] imageDtos)
    {
        return imageDtos.AsIterable().Traverse(dto => ProductImage.From(dto.Url, dto.AltText, dto.IsMain))
            .Map(pis => this with { ProductImages = ProductImages.Concat(pis).ToList() }).As();
    }

    public Product DeleteImages(IEnumerable<ProductImageId> ids)
    {

        return this with { ProductImages = ProductImages.Where(pi => !ids.Contains(pi.Id)).ToList() };
    }

    private Fin<Product> UpdateCategory(string category)
    {
        return Category.FromCode(category).Map(c => this with
        {
            Category = c,
            Sku = Sku.From(
                c.Code.ToString(),
                Color.Code.ToString(),
                Size.Code.ToString(),
                Brand.Code.ToString()
            )
        });
    }

    private Fin<Product> UpdateSize(string size)
    {
        return Size.FromCode(size).Map(s => this with
        {
            Size = s,
            Sku = Sku.From(
                Category.Code.ToString(),
                Color.Code.ToString(),
                s.Code.ToString(),
                Brand.Code.ToString()
            )
        });
    }

    private Fin<Product> UpdateBrand(string brand)
    {
        return Brand.FromCode(brand).Map(b => this with
        {
            Brand = b,
            Sku = Sku.From(
                Category.Code.ToString(),
                Color.Code.ToString(),
                Size.Code.ToString(),
                b.Code.ToString()
            )
        });
    }

    private Fin<Product> UpdateColor(string color)
    {
        return Color.FromCode(color).Map(c => this with
        {
            Color = c,
            Sku = Sku.From(
                Category.Code.ToString(),
                c.Code.ToString(),
                Size.Code.ToString(),
                Brand.Code.ToString()
            )
        });
    }

    private Fin<Product> UpdateDescription(string description)
    {
        return Description.From(description).Map(d => this with { Description = d });
    }

    private Product UpdateTotalSales(int soldItems = 1)
    {
        // should be called through an event when an order is completed
        var newTotal = TotalSales + soldItems;
        return this with { TotalSales = newTotal };
    }

    private Product UpdatePrice(decimal price)
    {
        AddDomainEvent(new ProductPriceChangedEvent(this, price));
        return this with { Price = Money.FromDecimal(price) };
    }

    private Product AddVariants(params Product[] products)
    {

        var newVariants = new List<Product>();
        foreach (var p in products)
        {
            if (Variants.Exists(v => v.Id == p.Id)) continue;
            newVariants.Add(p);
        }

        AddDomainEvent(new ProductVariantsAddedEvent(newVariants.Select(v => v.Id)));
        return this with { Variants = Variants.Concat(newVariants).ToList() };
    }

    public Product RemoveVariants(params ProductId[] variantIds)
    {
        return this with { Variants = [.. Variants.Where(v => !variantIds.Contains(v.Id))] };
    }

    public Product AddReview(Review review)
    {
        if (review.Rating <= Rating.Fair)
            AddDomainEvent(new ProductReviewAddedEvent(Id, review.UserId, review.Id, review.Rating));
        return this with
        {
            Reviews = Reviews.Append(review).ToList(),
        };
    }

    public Product UpdateReview(Review review)
    {
        var oldReview = Reviews.FirstOrDefault(r => r.Id == review.Id);
        if (oldReview is null)
            return this;

        var updatedReviews = Reviews
            .Where(r => r.Id != review.Id)
            .Append(review)
            .ToList();

        return this with
        {
            Reviews = updatedReviews
        };
    }

    public Product DeleteReview(Review review)
    {
        return this with
        {
            Reviews = Reviews.Where(r => r.Id != review.Id).ToList(),
        };
    }

    public Fin<Product> Update(
        UpdateProductDto dto, List<Product> variants, List<ImageResult> imageDtos)
    {
        var validationSeq = Seq<Fin<Product>>();

        if (Slug.Value != dto.Slug)
            validationSeq = validationSeq.Add(UpdateSlug(dto.Slug));
        if (Brand.Code.ToString() != dto.Brand)
            validationSeq = validationSeq.Add(UpdateBrand(dto.Brand));
        if (Color.Code.ToString() != dto.Color)
            validationSeq = validationSeq.Add(UpdateColor(dto.Color));
        if (Size.Code.ToString() != dto.Size)
            validationSeq = validationSeq.Add(UpdateSize(dto.Size));
        if (imageDtos.Any())
            validationSeq = validationSeq.Add(AddImages([.. imageDtos]));
        if (Category.Code.ToString() != dto.Category)
            validationSeq = validationSeq.Add(UpdateCategory(dto.Category));
        if (Description.Value != dto.Description)
            validationSeq = validationSeq.Add(UpdateDescription(dto.Description));

        if (Price.Value != dto.Price)
            UpdatePrice(dto.Price);

        Status.Update(dto.IsFeatured, dto.IsTrending, dto.IsBestSeller, dto.IsNew);
        if (variants.Any()) AddVariants([.. variants]);

        if (validationSeq.IsEmpty)
            return this;
        return validationSeq.Traverse(identity)
            .Map(seq => seq.Last.Match(p => p, () => this)).As();
    }
    public virtual bool Equals(Product? other)
    {
        return other is not null && Id == other.Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}