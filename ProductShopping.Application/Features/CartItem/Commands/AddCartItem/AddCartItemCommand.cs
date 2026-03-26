using MediatR;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Commands.AddCartItem;

public class AddCartItemCommand : IRequest<Result<GetCartItemDto>>
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
