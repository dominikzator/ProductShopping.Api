using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.DTOs.CartItem;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using System.Security.Claims;

namespace ProductShopping.Application.Services;

public class CartItemsService(ProductShoppingDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<CartItemsService> logger) : ICartItemsService
{
    public async Task<Result<PagedResult<GetCartItemDto>>> GetCartItemsAsync(PaginationParameters paginationParameters)
    {
        var userCart = await GetUserCart();

        var query = context.CartItems.AsQueryable();

        var cartItems = await query
            .Where(c => c.CartId == userCart.CartId)
            .Include(c => c.Product)
            .ThenInclude(p => p.Category).ToListAsync();

        var dtos = mapper.Map<List<GetCartItemDto>>(cartItems);

        var pagedResult = new PagedResult<GetCartItemDto>
        {
            Data = PagedResult<GetCartItemDto>.GetData(dtos, paginationParameters),
            Metadata = PagedResult<GetCartItemDto>.GetPaginationMetadata(dtos, paginationParameters)
        };

        return Result<PagedResult<GetCartItemDto>>.Success(pagedResult);
    }

    public async Task<Result<GetCartItemDto>> GetCartItemAsync(int id)
    {
        var userCart = await GetUserCart();

        var cartItem = await context.CartItems
            .Where(c => c.CartId == userCart.CartId && c.Id == id)
            .Include(c => c.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync();

        if (cartItem is null)
        {
            return Result<GetCartItemDto>.Failure(new Error(ErrorCodes.NotFound, $"CartItem '{id}' was not found."));
        }

        var outputDto = mapper.Map<GetCartItemDto>(cartItem);

        return Result<GetCartItemDto>.Success(outputDto);
    }

    public async Task<Result<GetCartItemDto>> AddCartItemToCartAsync(CreateCartItemDto createCartItemDto)
    {
        var userCart = await GetUserCart();

        if(userCart == null)
        {
            return Result<GetCartItemDto>.Failure(new Error(ErrorCodes.Failure, 
                $"User '{httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var product = await context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == createCartItemDto.ProductId);

        if(product == null)
        {
            return Result<GetCartItemDto>.Failure(new Error(ErrorCodes.NotFound, $"A Product with id: '{createCartItemDto.ProductId}' does not exists."));
        }

        var cartItem = context.CartItems.FirstOrDefault(cartItem => cartItem.ProductId == createCartItemDto.ProductId && cartItem.CartId == userCart.CartId);

        if (cartItem != null)
        {
            cartItem.Quantity += createCartItemDto.Quantity;
            context.CartItems.Update(cartItem);
        }
        else
        {
            cartItem = mapper.Map<CartItem>(createCartItemDto);
            cartItem.CartId = userCart.CartId;

            context.CartItems.Add(cartItem);
        }
        await context.SaveChangesAsync();

        var savedItem = await context.CartItems
            .Include(c => c.Product)
            .ThenInclude(c => c.Category)
            .FirstAsync(c => c.Id == cartItem.Id);

        var outputDto = mapper.Map<GetCartItemDto>(savedItem);

        return Result<GetCartItemDto>.Success(outputDto);
    }

    public async Task<Result> DeleteCartItemFromCartAsync(RemoveCartItemDto removeCartItemDto)
    {
        var userCart = await GetUserCart();

        if (userCart == null)
        {
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"User '{httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        var cartItem = context.CartItems.FirstOrDefault(cartItem => cartItem.Id == removeCartItemDto.CartItemId);

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
            context.CartItems.Update(cartItem);
        }
        else
        {
            context.CartItems.Remove(cartItem);
        }
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Cart?> GetUserCart() => await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId ==
                httpContextAccessor
                .HttpContext!
                .User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task<Result> ClearCartAsync()
    {
        var userCart = await GetUserCart();

        if (userCart == null)
        {
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"User '{httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        if (userCart.CartItems.Count == 0)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, $"User Cart is empty. There is nothing to delete."));
        }

        context.CartItems.RemoveRange(userCart.CartItems);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}
