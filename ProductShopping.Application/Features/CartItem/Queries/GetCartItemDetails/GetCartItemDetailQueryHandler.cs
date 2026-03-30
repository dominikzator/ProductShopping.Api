using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;

public class GetCartItemDetailQueryHandler(ICartsRepository cartsRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetCartItemDetailQuery, Result<CartItemDto>>
{
    public async Task<Result<CartItemDto>> Handle(GetCartItemDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        var cartItem = userCart.Value!.CartItems;

        if (cartItem is null)
        {
            throw new NotFoundException($"CartItem '{request.Id}' was not found.");
        }

        var outputDto = mapper.Map<CartItemDto>(cartItem);

        return Result<CartItemDto>.Success(outputDto);
    }
}