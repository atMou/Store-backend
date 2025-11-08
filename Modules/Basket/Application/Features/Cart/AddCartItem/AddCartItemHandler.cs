using Basket.Domain.Contracts;
using Basket.Domain.Models;

using MediatR;

using Shared.Application.Contracts.Product.Queries;
using Shared.Domain.Contracts.Cart;

namespace Basket.Application.Features.Cart.AddCartItem;

public record AddCartItemCommand(
    CartId CartId,
    ProductId ProductId,
    int Quantity)
    : ICommand<Fin<CreateCartItemCommandResult>>;

public record CreateCartItemCommandResult(CartDto CartDto);

internal class CreateCartItemCommandHandler(
    ISender sender,
    BasketDbContext dbContext,
    ICartRepository cartRepository
) : ICommandHandler<AddCartItemCommand, Fin<CreateCartItemCommandResult>>
{
    public async Task<Fin<CreateCartItemCommandResult>> Handle(AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var db = from cart in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.GetCartById(command.CartId, ctx, opts =>
                {
                    opts.AsSplitQuery = true;
                    opts.AddInclude(cart => cart.CartItems);
                }))
                 from res in IO.liftAsync(async e =>
                     await sender.Send(new GetProductByIdQuery(command.ProductId), e.Token))
                 let c = res.Match<CartDto?>(result => cart.AddCartItem(

                     CartItem.Create(new CreateCartItemDto
                     {
                         CartId = cart.Id.Value,
                         Slug = result.dto.Slug,
                         Sku = result.dto.Sku,
                         ProductId = result.dto.Id,
                         ImageUrl = result.dto.ImageUrls.FirstOrDefault()!,
                         Quantity = command.Quantity,
                         UnitPrice = result.dto.Price
                     }
            )).ToDto(), _ => null)


                 select new CreateCartItemCommandResult(c);

        return await db.RunSave(dbContext, EnvIO.New(null, cancellationToken));
    }
}