using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface ICartsRepository : IGenericRepository<Cart>
    {
        Task<Result<bool>> ClearCartAsync(string userId);
        Task<Result<bool>> CreateCartItemAsync(CartItem cartItem);
        Task<Result<bool>> DeleteCartItemAsync(CartItem cartItem);
        Task<Result<CartDto>> GetUserCartAsync(string userId);
        Task<Result<CartItemDto>> GetUserCartItemAsync(string userId, int cartItemId);
        Task<Result<CartItemDto>> GetUserCartItemByProductIdAsync(string userId, int productId);
        Task<Result<List<CartItemDto>>> GetUserCartItemsAsync(string userId);
        Task<Result<bool>> UpdateCartItemAsync(CartItem cartItem);
    }
}