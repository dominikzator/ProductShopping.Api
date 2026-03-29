using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItems;

public class GetCartItemListQueryHandler(ICartsRepository cartsRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetCartItemListQuery, Result<PagedResult<CartItemDto>>>
{
    public async Task<Result<PagedResult<CartItemDto>>> Handle(GetCartItemListQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        var cartItems = await cartsRepository.GetUserCartItemsAsync(userId);

        var dtos = mapper.Map<List<CartItemDto>>(cartItems.Value);

        var pagedResult = new PagedResult<CartItemDto>
        {
            Data = PagedResult<CartItemDto>.GetData(dtos, request.PaginationParameters!),
            Metadata = PagedResult<CartItemDto>.GetPaginationMetadata(dtos, request.PaginationParameters!)
        };

        return Result<PagedResult<CartItemDto>>.Success(pagedResult);
    }
}
