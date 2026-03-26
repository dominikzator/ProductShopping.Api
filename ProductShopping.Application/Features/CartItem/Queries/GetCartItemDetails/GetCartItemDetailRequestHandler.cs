using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;

public class GetCartItemDetailRequestHandler(ICartsRepository cartsRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetCartItemDetailQuery, Result<GetCartItemDto>>
{
    public async Task<Result<GetCartItemDto>> Handle(GetCartItemDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        var cartItem = cartsRepository.GetUserCartItemAsync(userId, request.Id);

        if (cartItem is null)
        {
            throw new NotFoundException($"CartItem '{request.Id}' was not found.");
        }

        var outputDto = mapper.Map<GetCartItemDto>(cartItem);

        return Result<GetCartItemDto>.Success(outputDto);
    }
}