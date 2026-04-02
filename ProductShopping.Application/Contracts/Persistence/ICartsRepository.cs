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
        Task<Cart> GetUserCartNoTrackingAsync(string userId);
        Task<Result<CartDto>> GetUserCartDtoAsync(string userId);
        Task<CartItem> GetUserCartItemAsync(string userId, int cartItemId, bool tracking = false);
        Task<CartItem> GetUserCartItemByProductIdNoTrackingAsync(string userId, int productId);
        Task<Result<CartItemDto>> GetUserCartItemDtoAsync(string userId, int cartItemId);
        Task<Result<CartItemDto>> GetUserCartItemDtoByProductIdAsync(string userId, int productId);
        Task<Result<List<CartItemDto>>> GetUserCartItemsDtosAsync(string userId);
        Task<Result<bool>> UpdateCartItemAsync(CartItem cartItem);
        Task<Cart> GetUserCartAsync(string userId);
        Task<CartItem> GetUserCartItemByProductIdAsync(string userId, int productId);
    }
}