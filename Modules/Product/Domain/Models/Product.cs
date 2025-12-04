namespace Product.Domain.Models;

//add-migration init -OutputDir Persistence/Migrations -context ProductDBContext -Project Product -StartUpProject Api
//add-migration init -OutputDir Persistence/Migrations -context BasketDBContext -Project Basket -StartUpProject Api
//add-migration init -OutputDir Persistence/Migrations -context IdentityDBContext -Project Identity -StartUpProject Api
//add-migration init -OutputDir Persistence/Data/Migrations -context OrderDbContext -Project Order -StartUpProject Api
public record Product : Aggregate<ProductId>
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
        Discount discount
    //IEnumerable<Variant> variants
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
        //Variants = variants;
    }

    public Slug Slug { get; private init; }
    public Brand Brand { get; private init; }
    public Category Category { get; private init; }
    public Discount Discount { get; private init; }
    public Money Price { get; private init; }
    public Description Description { get; private init; }
    public Status Status { get; }

    public int TotalReviews
    {
        get { return Reviews.Count(); }
        private set { }
    }

    public int TotalSales { get; private init; }
    public double AverageRating { get; private init; }
    public bool IsDeleted { get; private init; }
    public Money? NewPrice { get; private init; }
    public ProductId? ParentProductId { get; private init; }
    public Product? ParentProduct { get; private init; }
    public IEnumerable<Product> Alternatives { get; private init; } = [];
    public IEnumerable<Variant> Variants { get; private init; } = [];
    public IEnumerable<Color> Colors => Variants.Select(v => v.Color);
    public IEnumerable<Size> Sizes => Variants.Select(v => v.Size);
    public IEnumerable<Review> Reviews { get; private init; } = [];
    public IEnumerable<ProductImage> Images { get; private set; } = [];


    [NotMapped]
    public Rating Rating =>
        Rating.From(AverageRating)
            .Match(rating => rating, _ => Rating.None);

    public virtual bool Equals(Product? other)
    {
        return other is not null && Id == other.Id;
    }

    public static Fin<Product> Create(CreateProductDto dto)
    {
        //var variants = dto.Variants.AsIterable().Traverse(v =>
        //    Variant.Create(v.Color, v.Size, dto.Brand, dto.Category, v.Stock, v.StockLow,
        //        v.StockMid, v.StockHigh, v.Attributes)).Map(vs => vs.AsEnumerable()).As();
        return (
                Slug.From(dto.Slug),
                Brand.FromCode(dto.Brand),
                Category.From(dto.Category),
                Description.From(dto.Description),
                Discount.From(dto.Price, dto.NewPrice)
            //variants
            ).Apply((slug, brand, category, description, discount) => new Product(
                slug,
                brand,
                category,
                description,
                dto.Price,
                dto.NewPrice,
                discount
            )).As()
            .Map(p =>
            {
                p.AddDomainEvent(new ProductCreatedDomainEvent(p));
                return p;
            });
    }


    public Product MarkAsDeleted()
    {
        return this with { IsDeleted = true };
    }

    private Fin<Product> UpdateSlug(string slug)
    {
        return Slug.From(slug).Map(s => this with { Slug = s });
    }

    public Product AddImages(params ImageResult[] imageResult)
    {
        return this with
        {
            Images =
            imageResult.Select(result => ProductImage.FromUnsafe(result.Url, result.AltText, result.IsMain))
        };
    }

    public Product AddImages(VariantId variantId, params ImageResult[] imageResult)
    {
        var variant = Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null) return this;
        return this with { Variants = [.. Variants, variant.AddImages(imageResult)] };
    }


    public Fin<Product> UpdateImages(params UpdateProductImageDto[] dtos)
    {
        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<ProductImage>>();
        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {
            var productImage = Images.FirstOrDefault(img => img.Id == dto.ProductImageId);
            return productImage is not null
                ? current.Add(productImage.Update(dto.Url, dto.AltText, dto.IsMain))
                : current.Add(ProductImage.From(dto.Url, dto.AltText, dto.IsMain));
        });

        return result.Traverse(identity)
            .Map(imgs => this with { Images = imgs.AsEnumerable() }).As();
    }

    public Fin<Product> UpdateVariants(params UpdateVariantDto[] dtos)
    {
        var _dtos = Seq([.. dtos]);
        var validationSeq = Seq<Fin<Variant>>(); // Update type to Fin<Variant>
        var result = _dtos.Fold(validationSeq, (current, dto) =>
        {
            var variant = Variants.FirstOrDefault(v => v.Id == dto.VariantId);
            return variant is not null
                ? current.Add(variant.Update(dto, Category, Brand))
                : current.Add(Variant.Create(
                    new CreateVariantDto
                    {
                        Attributes = dto.Attributes.Select(uDto => new CreateAttributeDto
                        {
                            Name = uDto.Name,
                            Description = uDto.Description
                        }),
                        Color = dto.Color,
                        Stock = dto.Stock,
                        StockLow = dto.StockLow,
                        StockMid = dto.StockMid,
                        StockHigh = dto.StockHigh,
                        Size = dto.Size
                    }, Brand.Name, Category.Name));
        });

        return result.Traverse(identity)
            .Map(vs => this with { Variants = vs.AsEnumerable() }).As();
    }


    public Product AddVariants(IEnumerable<Variant> variants)
    {
        return this with { Variants = variants };
    }

    public Product DeleteImages(IEnumerable<ProductImageId> ids)
    {
        return this with { Images = Images.Where(pi => !ids.Contains(pi.Id)).ToList() };
    }

    private Fin<Product> UpdateCategory(string category)
    {
        return Category.From(category).Map(c => this with
        {
            Category = c
        }).Map(updatedProduct =>
            updatedProduct with
            {
                Variants = Variants.Select(v => v.UpdateSkuForCategory(Brand, updatedProduct.Category)).ToList()
            });
    }

    private Fin<Product> UpdateSize(string size, VariantId variantId)
    {
        return Optional(Variants.FirstOrDefault(v => v.Id == variantId))
            .ToFin(NotFoundError.New($"Product variant with id {variantId} not found."))
            .Bind(v => v.UpdateSize(size, Brand, Category)
                .Map(updatedVariant =>
                {
                    var updatedVariants = Variants
                        .Where(var => var.Id != variantId)
                        .Append(updatedVariant)
                        .ToList();
                    return this with { Variants = updatedVariants };
                })
            );
    }


    private Fin<Product> UpdateBrand(string brand)
    {
        return Brand.FromCode(brand).Map(b => this with
        {
            Brand = b,
            Variants = Variants.Select(v => v.UpdateSkuForBrand(b, Category))
        });
    }


    private Fin<Product> UpdateColor(string color, VariantId variantId)
    {
        return Optional(Variants.FirstOrDefault(v => v.Id == variantId))
            .ToFin(NotFoundError.New($"Product variant with id {variantId} not found."))
            .Bind(v => v.UpdateColor(color, Brand, Category)
                .Map(updatedVariant =>
                {
                    var updatedVariants = Variants
                        .Where(var => var.Id != variantId)
                        .Append(updatedVariant)
                        .ToList();
                    return this with { Variants = updatedVariants };
                })
            );
    }


    private Fin<Product> UpdateDescription(string description)
    {
        return Description.From(description).Map(d => this with { Description = d });
    }

    public Fin<Product> UpdateStock(VariantId variantId, int stock, int? low = null, int? mid = null, int? high = null)
    {
        return Optional(Variants.FirstOrDefault(v => v.Id == variantId))
            .ToFin(NotFoundError.New($"Product variant with id {variantId} not found."))
            .Bind(v => v.UpdateStock(stock, low, mid, high))
            .Map(updatedVariant =>
            {
                var updatedVariants = Variants
                    .Where(var => var.Id != variantId)
                    .Append(updatedVariant)
                    .ToList();
                return this with { Variants = updatedVariants };
            });
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

    private Product AddSimilarProducts(params Product[] products)
    {
        var newVariants = new List<Product>();
        foreach (var p in products)
        {
            if (Alternatives.FirstOrDefault(v => v.Id == p.Id) is not null)
            {
                continue;
            }

            newVariants.Add(p);
        }

        return this with { Alternatives = Alternatives.Concat(newVariants).ToList() };
    }

    public Product RemoveSimilarProducts(params Product[] products)
    {
        return this with { Alternatives = Alternatives.Where(v => !products.Contains(v)).ToList() };
    }

    public Product AddReview(Review review)
    {
        if (review.Rating <= Rating.Fair)
        {
            AddDomainEvent(new ProductReviewAddedEvent(Id, review.UserId, review.Id, review.Rating));
        }

        return this with
        {
            Reviews = Reviews.Append(review).ToList(),
            AverageRating = CalculateRating(review.Rating, Reviews.Select(r => r.Rating))
        };
    }

    public Product UpdateReview(Review review)
    {
        var oldReview = Reviews.FirstOrDefault(r => r.Id == review.Id);
        if (oldReview is null)
        {
            return this;
        }

        var updatedReviews = Reviews
            .Where(r => r.Id != review.Id)
            .Append(review)
            .ToList();

        return this with
        {
            Reviews = updatedReviews,
            AverageRating = updatedReviews.Any()
                ? updatedReviews.Average(r => r.Rating.Value)
                : 0
        };
    }

    public Product DeleteReview(Review review)
    {
        return this with
        {
            Reviews = Reviews.Where(r => r.Id != review.Id).ToList(),
            AverageRating = Reviews.Count() > 1
                ? Reviews.Average(r => r.Rating.Value)
                : 0
        };
    }

    private double CalculateRating(Rating rating, IEnumerable<Rating> ratings)
    {
        var rts = ratings.ToArray();
        if (!rts.Any())
        {
            return rating.Value;
        }

        var totalRating = rts.Sum(r => r.Value) + rating.Value;
        var averageRating = totalRating / (rts.Length + 1);
        return averageRating;
    }

    public Fin<Product> Update(
        UpdateProductDto dto,
        IEnumerable<Product> alternatives)
    {
        var validationSeq = Seq<Fin<Product>>();

        if (Slug.Value != dto.Slug)
        {
            validationSeq = validationSeq.Add(UpdateSlug(dto.Slug));
        }

        if (Brand.Code.ToString() != dto.Brand)
        {
            validationSeq = validationSeq.Add(UpdateBrand(dto.Brand));
        }

        if (Category.Code.ToString() != dto.Category)
        {
            validationSeq = validationSeq.Add(UpdateCategory(dto.Category));
        }

        if (Description.Value != dto.Description)
        {
            validationSeq = validationSeq.Add(UpdateDescription(dto.Description));
        }

        if (dto.Variants.Any())
        {
            validationSeq = validationSeq.Add(UpdateVariants([.. dto.Variants]));
        }

        if (Price.Value != dto.Price)
        {
            UpdatePrice(dto.Price);
        }

        if (dto.ImageDtos.Any())
        {
            validationSeq = validationSeq.Add(UpdateImages([.. dto.ImageDtos]));
        }

        if (dto.AlternativesIds.Any())
        {
            AddSimilarProducts([.. alternatives]);
        }

        Status.Update(dto.IsFeatured, dto.IsTrending, dto.IsBestSeller, dto.IsNew);


        if (validationSeq.IsEmpty)
        {
            return this;
        }

        return validationSeq.Traverse(identity)
            .Map(seq => seq.Last.IfNone(() => this)).As();
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}