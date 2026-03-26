using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;

public class RemoveCartItemCommand : IRequest<Result>
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
}
