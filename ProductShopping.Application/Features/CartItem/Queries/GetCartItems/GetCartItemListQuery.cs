using MediatR;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItems;

public class GetCartItemListQuery : IRequest<Result<PagedResult<CartItemDto>>>
{
    public PaginationParameters? PaginationParameters { get; set; }
}