using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Commands.AddCartItem;

public class AddCartItemCommandHandler(ICartsRepository cartsRepository, IProductsRepository productsRepository, IUsersService usersService, IMapper mapper)
    : IRequestHandler<AddCartItemCommand, Result<CartItemDto>>
{
    public async Task<Result<CartItemDto>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddCartItemCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new ValidationFailedException("Validation failed for creating a CartItem", validationResult);

        var userId = usersService.GetUserId();
        var userEmail = usersService.GetUserEmail();

        var userCart = await cartsRepository.GetUserCartAsync(userId);

        if (userCart == null)
        {
            return Result<CartItemDto>.Failure(new Error(ErrorCodes.Failure,
                $"User '{userEmail}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var product = await productsRepository.GetByIdAsync(request.ProductId);

        if(product is null)
        {
            throw new NotFoundException($"A Product with id: {request.ProductId} does not exist");
        }

        var cartItem = await cartsRepository.GetUserCartItemByProductIdAsync(userId, request.ProductId);

        if (cartItem != null)
        {
            cartItem.Quantity += request.Quantity;

            await cartsRepository.SaveChangesAsync();
        }
        else
        {
            cartItem = new Domain.Models.CartItem
            {
                ProductId = request.ProductId,
                CartId = userCart.Id,
                Quantity = request.Quantity,
            };

            await cartsRepository.CreateCartItemAsync(cartItem);
        }

        var changedCartItem = await cartsRepository.GetUserCartItemAsync(userId, cartItem.Id);

        var outputDto = mapper.Map<CartItemDto>(changedCartItem);

        return Result<CartItemDto>.Success(outputDto);
    }
}
