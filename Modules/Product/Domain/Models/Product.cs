using Product.Application.Events;
using Product.Domain.Contracts;

namespace Product.Domain.Models;

//add-migration init -OutputDir data/Migrations -Project Catalog -StartUpProject Api
public record Product : Aggregate<ProductId>
{
    private Product(Option<Money> newPrice) : base(ProductId.New())
    {
        NewPrice = newPrice;
    }

    private Product(
        Slug slug,
        Sku sku,
        List<ImageUrl> imageUrls,
        int stock,
        int lowStockThreshold,
        Money price,
        Currency currency,
        Brand brand,
        Size size,
        Color color,
        Category category,
        Description description,
        Option<Money> newPrice) : base(ProductId.New())
    {
        Slug = slug;
        Sku = sku;
        ProductStatus = ProductStatus.New;
        ImageUrls = imageUrls;
        Stock = stock;
        LowStockThreshold = lowStockThreshold;
        Price = price;
        Currency = currency;
        Brand = brand;
        Size = size;
        Color = color;
        Category = category;
        Description = description;
        NewPrice = newPrice;
    }

    public Slug Slug { get; private init; }
    public Sku Sku { get; }
    public ProductStatus ProductStatus { get; private init; }
    public List<ImageUrl> ImageUrls { get; private init; }
    public int Stock { get; private init; }
    public int LowStockThreshold { get; private init; }
    public Currency Currency { get; }
    public Brand Brand { get; private init; }
    public Color Color { get; private init; }
    public Size Size { get; }
    public Category Category { get; private init; }
    public Description Description { get; private init; }
    public int TotalReviews { get; private init; }
    public int TotalSales { get; private init; }
    public List<Product> Variants { get; private init; } = [];
    public List<Review> Reviews { get; private init; } = [];
    public Money Price { get; private init; }
    private double _averageRating { get; init; }

    private Money? _newPrice
    {
        get
        {
            return NewPrice.Match<Money?>(
                np => np,
                () => null
            );
        }
    }

    [NotMapped] public bool IsLowStock => Stock <= LowStockThreshold;

    [NotMapped] public Option<Money> NewPrice { get; }

    [NotMapped]
    public Option<Money> Discount
    {
        get
        {
            return NewPrice switch
            {
                { IsNone: true } => Option<Money>.None,
                { IsSome: true } np => np.Bind(npValue => Price.Value > npValue.Value
                    ? Some(Money.FromDecimal(Price.Value - npValue.Value))
                    : Option<Money>.None),
                _ => Option<Money>.None
            };
        }
    }

    [NotMapped]
    public Rating AvgRating =>
        Rating.From(_averageRating)
            .Match(rating => rating, _ => Rating.None);


    public static Fin<Product> Create(CreateProductDto dto)
    {
        var images = dto.ImageUrls.AsIterable().Traverse(ImageUrl.From).Map(imgs => imgs.ToList()).As();
        return (
                Slug.From(dto.Slug),
                images,
                Currency.FromCode(dto.Currency),
                Brand.FromCode(dto.Brand),
                Size.FromCode(dto.Size),
                Color.FromCode(dto.Color),
                Category.FromCode(dto.Category),
                Description.From(dto.Description)
            ).Apply((_slug, _url, _curr, _brand, _size, _color, _cat, _desc) =>
                new Product(
                    _slug,
                    Sku.From(
                        _cat.Code.ToString(),
                        _color.Code.ToString(),
                        _size.Code.ToString(),
                        _brand.Code.ToString()
                    ),
                    _url,
                    dto.Stock,
                    dto.LowStockThreshold,
                    Money.FromDecimal(dto.Price),
                    _curr,
                    _brand,
                    _size,
                    _color,
                    _cat,
                    _desc,
                    Optional(dto.NewPrice).Map(Money.FromDecimal)
                )).As();
        //.Map(p =>
        //{
        //    p.AddDomainEvent(new ProductCreatedEvent(p.Category.Code.ToString()));
        //    return p;
        //});
    }


    public Fin<Product> UpdateSlug(string slug)
    {
        return Slug.From(slug).Map(s => this with { Slug = s });
    }

    public Fin<Product> AddImages(string[] imageUrls)
    {
        return imageUrls.AsIterable().Traverse(ImageUrl.From)
            .Map(urls => urls.Where(url => ImageUrls.Exists(imageUrl => url != imageUrl)))
            .Map(imgUrls => this with { ImageUrls = ImageUrls.Concat(imgUrls).ToList() }).As();
    }

    public Fin<Product> DeleteImages(string[] imageUrls)
    {
        return imageUrls.AsIterable().Traverse(ImageUrl.From)
            .Map(imgUrls => this with { ImageUrls = ImageUrls.Except(imgUrls).ToList() }).As();
    }

    public Fin<Product> UpdateBrand(string brand)
    {
        //AddDomainEvent(new BrandChangedEvend()); // This one should trigger slug update as well
        return Brand.FromCode(brand).Map(b => this with { Brand = b });
    }


    public Fin<Product> UpdateColor(string color)
    {
        //AddDomainEvent(new BrandChangedEvend()); // This one should trigger slug update as well
        return Color.FromCode(color).Map(c => this with { Color = c });
    }

    public Fin<Product> UpdateCategory(string category)
    {
        //AddDomainEvent(new BrandChangedEvend()); // This one should trigger slug update as well
        return Category.FromCode(category).Map(c => this with { Category = c });
    }

    public Fin<Product> UpdateDescription(string description)
    {
        return Description.From(description).Map(d => this with { Description = d });
    }

    public Product UpdateTotalSales(int soldItems = 1)
    {
        var newTotal = TotalSales + soldItems;
        return this with { TotalSales = newTotal };
    }

    public Product UpdatePrice(decimal price)
    {
        AddDomainEvent(new ProductPriceChangedEvent(Id, price));
        return this with { Price = Money.FromDecimal(price) };
    }

    // Price Changed Domain Event and invalidate cache
    public Product UpdateStock(int stock)
    {
        return this with { Stock = stock };
        // Stock Changed Domain Event and invalidate cache
    }

    public Product UpdateLowStockThreshold(int threshold)
    {
        return this with { LowStockThreshold = threshold };
        // Threshold Changed Domain Event and invalidate cache
    }


    public Product AddVariant(Product product)
    {
        return this with { Variants = Variants.Append(product).ToList() };
    }

    public Product RemoveVariant(ProductId variantId)
    {
        return this with { Variants = Variants.Where(v => v.Id != variantId).ToList() };
    }

    public Product AddReview(Review review)
    {
        var newTotal = TotalReviews + 1;
        var newAvg = (_averageRating * TotalReviews + review.Rating.Value) / newTotal;

        return this with
        {
            Reviews = Reviews.Append(review).ToList(),
            TotalReviews = newTotal,
            _averageRating = newAvg
        };
    }

    public Product UpdateSoldTimes(int times = 1) // should be called through a n event when an order is completed  
    {
        var newTimesSold = TotalSales + times;
        return this with { TotalSales = newTimesSold };
    }


    public Fin<Product> Update(
        UpdateProductDto dto
    )
    {
        var validationSeq = Seq<Fin<Product>>();

        if (Slug.Value != dto.Slug)
            validationSeq = validationSeq.Add(UpdateSlug(dto.Slug));
        if (dto.ImageUrls.Length > 0)
            validationSeq = validationSeq.Add(AddImages(dto.ImageUrls));
        if (Category.Code.ToString() != dto.Category)
            validationSeq = validationSeq.Add(UpdateCategory(dto.Category));
        if (Description.Value != dto.Description)
            validationSeq = validationSeq.Add(UpdateDescription(dto.Description));
        if (Stock != dto.Stock)
            UpdateStock(dto.Stock);
        if (LowStockThreshold != dto.LowStockThreshold)
            UpdateLowStockThreshold(dto.LowStockThreshold);
        if (Price.Value != dto.Price)
            UpdatePrice(dto.Price);
        ProductStatus.Update(dto.IsFeatured, dto.IsTrending, dto.IsBestSeller, dto.IsNew);

        if (validationSeq.IsEmpty)
            return this;
        return validationSeq.Traverse(identity)
            .Map(_ => this).As();
    }

    public void Deconstruct(out Slug slug, out Sku sku, out bool isNew, out bool isFeatured, out bool isBestSeller,
        out bool isTrending, out ImageUrl[] imageUrl,
        out int stock, out int lowStockThreshold, out Money price, out Brand brand,
        out Color color, out Size size, out Category category, out Description description)
    {
        slug = Slug;
        sku = Sku;
        isNew = ProductStatus.IsNew;
        isFeatured = ProductStatus.IsFeatured;
        isBestSeller = ProductStatus.IsBestSeller;
        isTrending = ProductStatus.IsTrending;
        imageUrl = ImageUrls.ToArray();
        stock = Stock;
        lowStockThreshold = LowStockThreshold;
        price = Price;
        brand = Brand;
        color = Color;
        size = Size;
        category = Category;
        description = Description;
    }
}