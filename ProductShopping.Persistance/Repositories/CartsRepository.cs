using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Persistence.Repositories;

public class CartsRepository : GenericRepository<Cart>, ICartsRepository
{
    private readonly ProductShoppingDbContext _context;
    private readonly IMapper _mapper;

    public CartsRepository(ProductShoppingDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CartDto>> GetUserCartDtoAsync(string userId)
    {
        var userCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(cart => cart.UserId == userId);

        if (userCart == null)
        {
            return Result<CartDto>.Failure(new Error(ErrorCodes.NotFound, $"User Cart does not exist."));
        }

        var userCartDto = _mapper.Map<CartDto>(userCart);

        return Result<CartDto>.Success(userCartDto);
    }

    public async Task<Cart> GetUserCartNoTrackingAsync(string userId)
    {
        var userCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(cart => cart.UserId == userId);

        return userCart;
    }

    public async Task<Cart> GetUserCartAsync(string userId)
    {
        var userCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .AsTracking()
            .FirstOrDefaultAsync(cart => cart.UserId == userId);

        return userCart;
    }

    public async Task<Result<List<CartItemDto>>> GetUserCartItemsDtosAsync(string userId)
    {
        var userCart = await GetUserCartDtoAsync(userId);
        var cartItems = userCart.Value!.CartItems.ToList();

        return Result<List<CartItemDto>>.Success(cartItems);
    }

    public async Task<List<CartItem>> GetUserCartItemsAsync(string userId)
    {
        var userCart = await GetUserCartNoTrackingAsync(userId);
        var cartItems = userCart.CartItems.ToList();
        Console.WriteLine($"{userCart.CartItems.Count}");

        return cartItems;
    }

    public async Task<Result<CartItemDto>> GetUserCartItemDtoAsync(string userId, int cartItemId)
    {
        var userCart = await GetUserCartDtoAsync(userId);
        var cartItem = userCart.Value!.CartItems.FirstOrDefault(c => c.Id == cartItemId);

        return Result<CartItemDto>.Success(cartItem!);
    }

    public async Task<CartItem> GetUserCartItemAsync(string userId, int cartItemId, bool tracking = false)
    {
        var userCart = (tracking) ? await GetUserCartAsync(userId) : await GetUserCartNoTrackingAsync(userId);
        var cartItem = userCart.CartItems.FirstOrDefault(c => c.Id == cartItemId);

        return cartItem;
    }

    public async Task<Result<CartItemDto>> GetUserCartItemDtoByProductIdAsync(string userId, int productId)
    {
        var userCart = await GetUserCartDtoAsync(userId);
        var cartItem = userCart.Value!.CartItems.FirstOrDefault(c => c.ProductId == productId);

        return Result<CartItemDto>.Success(cartItem!);
    }

    public async Task<CartItem> GetUserCartItemByProductIdNoTrackingAsync(string userId, int productId)
    {
        var userCart = await GetUserCartNoTrackingAsync(userId);
        var cartItem = userCart.CartItems.FirstOrDefault(c => c.ProductId == productId);

        return cartItem;
    }

    public async Task<CartItem> GetUserCartItemByProductIdAsync(string userId, int productId)
    {
        var userCart = await GetUserCartAsync(userId);
        var cartItem = userCart.CartItems.AsQueryable().AsTracking().FirstOrDefault(c => c.ProductId == productId);

        return cartItem;
    }

    public async Task<Result<bool>> CreateCartItemAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task DeleteCartItemAsync(string userId, int cartItemId)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (cartItem is null)
            return;

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<bool>> ClearCartAsync(string userId)
    {
        var cartItemsDtos = await GetUserCartItemsDtosAsync(userId);
        var cartItems = _mapper.Map<List<CartItem>>(cartItemsDtos.Value);

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}