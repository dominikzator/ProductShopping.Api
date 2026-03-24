using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
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

    public async Task<Result<Cart>> GetUserCartAsync(string userId)
    {
        var userCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(cart => cart.UserId == userId);

        if (userCart == null)
        {
            return Result<Cart>.Failure(new Error(ErrorCodes.NotFound, $"User Cart does not exist."));
        }

        return Result<Cart>.Success(userCart);
    }

    public async Task<Result<List<CartItem>>> GetUserCartItemsAsync(string userId)
    {
        var userCart = await GetUserCartAsync(userId);
        var cartItems = userCart.Value!.CartItems.ToList();

        return Result<List<CartItem>>.Success(cartItems);
    }

    public async Task<Result<CartItem>> GetUserCartItemAsync(string userId, int cartItemId)
    {
        var userCart = await GetUserCartAsync(userId);
        var cartItem = userCart.Value!.CartItems.FirstOrDefault(c => c.Id == cartItemId);

        return Result<CartItem>.Success(cartItem!);
    }

    public async Task<Result<CartItem>> GetUserCartItemByProductIdAsync(string userId, int productId)
    {
        var userCart = await GetUserCartAsync(userId);
        var cartItem = userCart.Value!.CartItems.FirstOrDefault(c => c.ProductId == productId);

        return Result<CartItem>.Success(cartItem!);
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

    public async Task<Result<bool>> ClearCartAsync(string userId)
    {
        var cartItems = await GetUserCartItemsAsync(userId);

        _context.CartItems.RemoveRange(cartItems.Value!);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}