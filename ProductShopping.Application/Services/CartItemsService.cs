using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.CartItem;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using System.Security.Claims;

namespace ProductShopping.Application.Services;

public class CartItemsService(ICartsRepository cartsRepository, IUsersService usersService,
    IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<CartItemsService> logger) : ICartItemsService
{
    public async Task<Result<PagedResult<GetCartItemDto>>> GetCartItemsAsync(PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        var cartItems = await cartsRepository.GetUserCartItemsAsync(userId);

        var dtos = mapper.Map<List<GetCartItemDto>>(cartItems.Value);

        var pagedResult = new PagedResult<GetCartItemDto>
        {
            Data = PagedResult<GetCartItemDto>.GetData(dtos, paginationParameters),
            Metadata = PagedResult<GetCartItemDto>.GetPaginationMetadata(dtos, paginationParameters)
        };

        return Result<PagedResult<GetCartItemDto>>.Success(pagedResult);
    }

    public async Task<Result<GetCartItemDto>> GetCartItemAsync(int cartItemId)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        var cartItem = cartsRepository.GetUserCartItemAsync(userId, cartItemId);

        if (cartItem is null)
        {
            return Result<GetCartItemDto>.Failure(new Error(ErrorCodes.NotFound, $"CartItem '{cartItemId}' was not found."));
        }

        var outputDto = mapper.Map<GetCartItemDto>(cartItem);

        return Result<GetCartItemDto>.Success(outputDto);
    }

    public async Task<Result<GetCartItemDto>> AddCartItemToCartAsync(CreateCartItemDto createCartItemDto)
    {
        var userId = usersService.GetUserId();
        var userEmail = usersService.GetUserEmail();

        var userCart = await cartsRepository.GetUserCartAsync(userId);

        if (userCart.Value == null)
        {
            return Result<GetCartItemDto>.Failure(new Error(ErrorCodes.Failure, 
                $"User '{userEmail}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var cartItemResult = await cartsRepository.GetUserCartItemByProductIdAsync(userId, createCartItemDto.ProductId);

        CartItem cartItem = cartItemResult.Value!;

        if (cartItem != null)
        {
            cartItem.Quantity += createCartItemDto.Quantity;

            await cartsRepository.UpdateCartItemAsync(cartItem);
        }
        else
        {
            cartItem = mapper.Map<CartItem>(createCartItemDto);
            cartItem.CartId = userCart.Value.Id;

            await cartsRepository.CreateCartItemAsync(cartItem);
        }

        var savedItem = await cartsRepository.GetUserCartItemAsync(userId, cartItem.Id);
        var outputDto = mapper.Map<GetCartItemDto>(savedItem.Value);

        return Result<GetCartItemDto>.Success(outputDto);
    }

    public async Task<Result> DeleteCartItemFromCartAsync(RemoveCartItemDto removeCartItemDto)
    {
        var userId = usersService.GetUserId();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

        if (userCart.Value == null)
        {
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"User '{httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var cartItemResult = await cartsRepository.GetUserCartItemAsync(userId, removeCartItemDto.CartItemId);

        var cartItem = cartItemResult.Value;

        if (cartItem == null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"A Cart Item with id: '{removeCartItemDto.CartItemId}' does not exists in your Cart."));
        }

        if (removeCartItemDto.Quantity > cartItem.Quantity)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"You are trying to remove more items than You have in your Cart. This should not happen. Please specify different Quantity"));
        }
        else if (removeCartItemDto.Quantity < cartItem.Quantity)
        {
            cartItem.Quantity -= removeCartItemDto.Quantity;

            await cartsRepository.UpdateCartItemAsync(cartItem);
        }
        else
        {
            await cartsRepository.DeleteCartItemAsync(cartItem);
        }

        return Result.Success();
    }

    public async Task<Result> ClearCartAsync()
    {
        var userId = usersService.GetUserId();
        var userEmail = usersService.GetUserEmail();
        var userCart = await cartsRepository.GetUserCartAsync(userId);

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
