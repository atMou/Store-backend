using Shared.Persistence;

namespace Product.Persistence.Data.Seeder;

public class ProductDataSeeder(ProductDBContext dbContext) : IDataSeeder
{
    public async Task<Unit> SeedAsync()
    {
        await dbContext.Products.AddRangeAsync(GetProductsSeed());
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return await Task.FromResult(unit);
    }

    public async Task<bool> HasDataAsync<TContext>(TContext db) where TContext : DbContext => await db.Set<Domain.Models.Product>().AnyAsync();

    public List<Domain.Models.Product> GetProductsSeed()
    {

        Seq<Fin<Domain.Models.Product>> finProducts =
        [
            //Domain.Models.Product.Create("test-product-1",  "https://example.com/image1.jpg", 100, 10, 29.99m,
            //    Currency.USD.Code, Brand.Adidas.Code.ToString(), Size.XXXL.Code.ToString(), Color.Blue.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 1 A great product 1 A great product 1 A great product 1."),
            //Domain.Models.Product.Create("test-product-2", "TP002", false,g(), Size.Medium.Code.ToString(), Color.Red.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 2. A great product 2 A great product 2 A great product 2."),


            #region commented
            //Product.Create("test-product-3", "TP003", true, "https://example.com/image3.jpg", 200, 20, 19.99m,
            //    Currency.GBP.Value, Brand.Puma.Code.ToString(), Size.Large.Code.ToString(), Color.Green.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 3.", true),
            //Product.Create("test-product-4", "TP004", false, "https://example.com/image4.jpg", 80, 8, 39.99m,
            //    Currency.USD.Value, Brand.Adidas.Code.ToString(), Size.Small.Code.ToString(),
            //    Color.Black.Code.ToString(), Category.Women.Code.ToString(), "A great product 4.", false),
            //Product.Create("test-product-5", "TP005", true, "https://example.com/image5.jpg", 120, 15, 59.99m,
            //    Currency.EUR.Value, Brand.Nike.Code.ToString(), Size.Medium.Code.ToString(),
            //    Color.White.Code.ToString(), Category.Women.Code.ToString(), "A great product 5.", true),
            //Product.Create("test-product-6", "TP006", false, "https://example.com/image6.jpg", 70, 7, 24.99m,
            //    Currency.GBP.Value, Brand.Puma.Code.ToString(), Size.Large.Code.ToString(), Color.Blue.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 6.", false),
            //Product.Create("test-product-7", "TP007", true, "https://example.com/image7.jpg", 150, 12, 34.99m,
            //    Currency.USD.Value, Brand.Adidas.Code.ToString(), Size.Small.Code.ToString(), Color.Red.Code.ToString(),
            //    Category.Kids.Code.ToString(), "A great product 7.", true),
            //Product.Create("test-product-8", "TP008", false, "https://example.com/image8.jpg", 60, 6, 44.99m,
            //    Currency.EUR.Value, Brand.Nike.Code.ToString(), Size.Medium.Code.ToString(),
            //    Color.Green.Code.ToString(), Category.Kids.Code.ToString(), "A great product 8.", false),
            //Product.Create("test-product-9", "TP009", true, "https://example.com/image9.jpg", 90, 9, 54.99m,
            //    Currency.GBP.Value, Brand.Puma.Code.ToString(), Size.Large.Code.ToString(), Color.Black.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 9.", true),
            //Product.Create("test-product-10", "TP010", false, "https://example.com/image10.jpg", 130, 13, 64.99m,
            //    Currency.USD.Value, Brand.Adidas.Code.ToString(), Size.XXL.Code.ToString(), Color.White.Code.ToString(),
            //    Category.Women.Code.ToString(), "A great product 10.", false),
            //Product.Create("test-product-11", "TP011", true, "https://example.com/image11.jpg", 110, 11, 27.99m,
            //    Currency.EUR.Value, Brand.Nike.Code.ToString(), Size.Small.Code.ToString(), Color.Blue.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 11.", true),
            //Product.Create("test-product-12", "TP012", false, "https://example.com/image12.jpg", 55, 5, 37.99m,
            //    Currency.GBP.Value, Brand.Puma.Code.ToString(), Size.Medium.Code.ToString(), Color.Red.Code.ToString(),
            //    Category.Kids.Code.ToString(), "A great product 12.", false),
            //Product.Create("test-product-13", "TP013", true, "https://example.com/image13.jpg", 140, 14, 47.99m,
            //    Currency.USD.Value, Brand.Adidas.Code.ToString(), Size.Large.Code.ToString(),
            //    Color.Green.Code.ToString(), Category.Women.Code.ToString(), "A great product 13.", true),
            //Product.Create("test-product-14", "TP014", false, "https://example.com/image14.jpg", 75, 7, 57.99m,
            //    Currency.EUR.Value, Brand.Nike.Code.ToString(), Size.Small.Code.ToString(), Color.Black.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 14.", false),
            //Product.Create("test-product-15", "TP015", true, "https://example.com/image15.jpg", 95, 9, 67.99m,
            //    Currency.GBP.Value, Brand.Puma.Code.ToString(), Size.Medium.Code.ToString(),
            //    Color.White.Code.ToString(), Category.Kids.Code.ToString(), "A great product 15.", true),
            //Product.Create("test-product-16", "TP016", false, "https://example.com/image16.jpg", 85, 8, 77.99m,
            //    Currency.USD.Value, Brand.Adidas.Code.ToString(), Size.Large.Code.ToString(),
            //    Color.Blue.Code.ToString(), Category.Women.Code.ToString(), "A great product 16.", false),
            //Product.Create("test-product-17", "TP017", true, "https://example.com/image17.jpg", 125, 12, 87.99m,
            //    Currency.EUR.Value, Brand.Nike.Code.ToString(), Size.Small.Code.ToString(), Color.Red.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 17.", true),
            //Product.Create("test-product-18", "TP018", false, "https://example.com/image18.jpg", 65, 6, 97.99m,
            //    Currency.GBP.Value, Brand.Puma.Code.ToString(), Size.Medium.Code.ToString(),
            //    Color.Green.Code.ToString(), Category.Kids.Code.ToString(), "A great product 18.", false),
            //Product.Create("test-product-19", "TP019", true, "https://example.com/image19.jpg", 105, 10, 107.99m,
            //    Currency.USD.Value, Brand.Adidas.Code.ToString(), Size.Large.Code.ToString(),
            //    Color.Black.Code.ToString(), Category.Women.Code.ToString(), "A great product 19.", true),
            //Product.Create("test-product-20", "TP020", false, "https://example.com/image20.jpg", 115, 11, 117.99m,
            //    Currency.EUR.Value, Brand.Nike.Code.ToString(), Size.Small.Code.ToString(), Color.White.Code.ToString(),
            //    Category.Men.Code.ToString(), "A great product 20.", false)
            #endregion
        ];

        return finProducts
            .Traverse(fin => fin.Match(Some, _ => Option<Domain.Models.Product>.None)).As().Match(seq => seq, () =>
            {
                Console.WriteLine($"Product seeding failed");
                return [];
            }).ToList();


    }
}
