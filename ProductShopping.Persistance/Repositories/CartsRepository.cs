using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Logging;
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
    private readonly IAppLogger<CartsRepository> _logger;

    public CartsRepository(ProductShoppingDbContext context, IMapper mapper, IAppLogger<CartsRepository> logger) : base(context)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CartDto>> GetUserCartAsync(string userId)
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

    public async Task<Result<List<CartItemDto>>> GetUserCartItemsAsync(string userId)
    {
        var userCart = await GetUserCartAsync(userId);
        _logger.LogInformation($"GetUserCartItemsAsync userCart items count: {userCart.Value.CartItems.Count}");
        var cartItems = userCart.Value!.CartItems.ToList();

        return Result<List<CartItemDto>>.Success(cartItems);
    }

    public async Task<Result<CartItemDto>> GetUserCartItemAsync(string userId, int cartItemId)
    {
        var userCart = await GetUserCartAsync(userId);
        var cartItem = userCart.Value!.CartItems.FirstOrDefault(c => c.Id == cartItemId);

        return Result<CartItemDto>.Success(cartItem!);
    }

    public async Task<Result<CartItemDto>> GetUserCartItemByProductIdAsync(string userId, int productId)
    {
        var userCart = await GetUserCartAsync(userId);
        var cartItem = userCart.Value!.CartItems.FirstOrDefault(c => c.ProductId == productId);

        return Result<CartItemDto>.Success(cartItem!);
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
        var cartItemsDtos = await GetUserCartItemsAsync(userId);
        var cartItems = _mapper.Map<List<CartItem>>(cartItemsDtos.Value);

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}