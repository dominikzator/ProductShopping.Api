using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;

public class GetCartItemDetailQuery : IRequest<Result<GetCartItemDto>>
{
    public int Id { get; set; }
}
