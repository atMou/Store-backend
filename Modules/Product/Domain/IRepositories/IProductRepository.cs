using Product.Application.Features.GetProducts;

namespace Product.Domain.IRepositories;

public interface IProductRepository
{
    IO<Models.Product> GetProductById(ProductId productId, ProductDBContext ctx, Action<QueryOptions<Models.Product>>? options = null);

    IO<(List<Domain.Models.Product> Products, int TotalCount)> GetProducts(ProductDBContext ctx,
        GetProductsQuery query);

    IO<Domain.Models.Product> UpdateProduct(UpdateProductDto productDto,
        ProductDBContext ctx, Action<QueryOptions<Domain.Models.Product>>? options = null);

    public IO<bool> DeleteProduct(ProductId productId, ProductDBContext ctx);
    public IO<ProductId> UpdateProductPrice(ProductId productId, decimal newPrice, ProductDBContext ctx);


}
