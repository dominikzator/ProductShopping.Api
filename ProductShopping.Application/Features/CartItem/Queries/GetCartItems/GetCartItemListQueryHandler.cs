using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItems;

public class GetCartItemListQueryHandler(ICartsRepository cartsRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetCartItemListQuery, Result<PagedResult<GetCartItemDto>>>
{
    public async Task<Result<PagedResult<GetCartItemDto>>> Handle(GetCartItemListQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        var cartItems = await cartsRepository.GetUserCartItemsAsync(userId);

        var dtos = mapper.Map<List<GetCartItemDto>>(cartItems.Value);

        var pagedResult = new PagedResult<GetCartItemDto>
        {
            Data = PagedResult<GetCartItemDto>.GetData(dtos, request.PaginationParameters!),
            Metadata = PagedResult<GetCartItemDto>.GetPaginationMetadata(dtos, request.PaginationParameters!)
        };

        return Result<PagedResult<GetCartItemDto>>.Success(pagedResult);
    }
}
