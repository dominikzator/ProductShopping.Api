using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Commands.RemoveCartItems;

public class RemoveCartItemsCommandHandler(ICartsRepository cartsRepository, IUsersService usersService)
    : IRequestHandler<RemoveCartItemsCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemsCommand request, CancellationToken cancellationToken)
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

        if (userCart.Value.CartItems.Count == 0)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, $"User Cart is empty. There is nothing to delete."));
        }

        await cartsRepository.ClearCartAsync(userId);

        return Result.Success();
    }
}
