using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Results;
using System.Security.Claims;

namespace ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler(ICartsRepository cartsRepository, IUsersService usersService, IMapper mapper)
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();
        var userEmail = usersService.GetUserEmail();
        var userCart = await cartsRepository.GetUserCartDtoAsync(userId);

        if (userCart.Value == null)
        {
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"User '{userEmail}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var cartItemDtoResult = await cartsRepository.GetUserCartItemDtoAsync(userId, request.CartItemId);

        if(cartItemDtoResult.Value == null)
        {
            throw new NotFoundException($"CartItem with id: {request.CartItemId} not found");
        }

        var cartItem = mapper.Map<Domain.Models.CartItem>(cartItemDtoResult.Value);

        if (cartItem == null)
        {
            throw new NotFoundException($"CartItem with id: {request.CartItemId} not found");
        }

        if (request.Quantity > cartItem.Quantity)
        {
            throw new NotFoundException($"You are trying to remove more items than You have in your Cart. This should not happen. Please specify different Quantity");
        }
        else if (request.Quantity < cartItem.Quantity)
        {
            cartItem.Quantity -= request.Quantity;

            await cartsRepository.UpdateCartItemAsync(cartItem);
        }
        else
        {
            await cartsRepository.DeleteCartItemAsync(cartItem);
        }

        return Result.Success();
    }
}
