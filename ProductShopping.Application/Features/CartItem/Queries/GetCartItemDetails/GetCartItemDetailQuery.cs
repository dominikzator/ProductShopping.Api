using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;

public class GetCartItemDetailQuery : IRequest<Result<CartItemDto>>
{
    public int Id { get; set; }
}
