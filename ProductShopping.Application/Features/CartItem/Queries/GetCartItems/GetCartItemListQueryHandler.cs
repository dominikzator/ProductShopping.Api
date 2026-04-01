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
        var userCart = await cartsRepository.GetUserCartDtoAsync(userId);

        var cartItemsDtos = await cartsRepository.GetUserCartItemsDtosAsync(userId);

        var pagedResult = new PagedResult<CartItemDto>
        {
            Data = PagedResult<CartItemDto>.GetData(cartItemsDtos.Value, request.PaginationParameters!),
            Metadata = PagedResult<CartItemDto>.GetPaginationMetadata(cartItemsDtos.Value, request.PaginationParameters!)
        };

        return Result<PagedResult<CartItemDto>>.Success(pagedResult);
    }
}
