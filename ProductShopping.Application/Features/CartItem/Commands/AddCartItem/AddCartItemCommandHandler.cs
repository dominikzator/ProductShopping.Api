using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.CartItem.Commands.AddCartItem;

public class AddCartItemCommandHandler(ICartsRepository cartsRepository, IProductsRepository productsRepository, IUsersService usersService, IMapper mapper, IAppLogger<AddCartItemCommandHandler> logger) 
    : IRequestHandler<AddCartItemCommand, Result<CartItemDto>>
{
    public async Task<Result<CartItemDto>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddCartItemCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new ValidationFailedException("Validation failed for creating a CartItem");

        var userId = usersService.GetUserId();
        var userEmail = usersService.GetUserEmail();

        var userCart = await cartsRepository.GetUserCartAsync(userId);

        if (userCart.Value == null)
        {
            return Result<CartItemDto>.Failure(new Error(ErrorCodes.Failure,
                $"User '{userEmail}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var product = await productsRepository.GetByIdAsync(request.ProductId);

        if(product is null)
        {
            return Result<CartItemDto>.Failure(new Error(ErrorCodes.NotFound, $"A Product with id: {request.ProductId} does not exist"));
        }

        var cartItemDtoResult = await cartsRepository.GetUserCartItemByProductIdAsync(userId, request.ProductId);
        var cartItem = mapper.Map<Domain.Models.CartItem>(cartItemDtoResult.Value);

        if (cartItem != null)
        {
            cartItem.Quantity += request.Quantity;

            await cartsRepository.UpdateCartItemAsync(cartItem);
        }
        else
        {
            cartItem = mapper.Map<Domain.Models.CartItem>(request);
            cartItem.CartId = userCart.Value.Id;

            await cartsRepository.CreateCartItemAsync(cartItem);
        }

        var savedItem = await cartsRepository.GetUserCartItemAsync(userId, cartItem.Id);
        var outputDto = mapper.Map<CartItemDto>(savedItem.Value);

        return Result<CartItemDto>.Success(outputDto);
    }
}
