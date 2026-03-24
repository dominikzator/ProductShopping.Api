using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface ICartsRepository
    {
        Task<Result<bool>> ClearCartAsync(string userId);
        Task<Result<bool>> CreateCartItemAsync(CartItem cartItem);
        Task<Result<bool>> DeleteCartItemAsync(CartItem cartItem);
        Task<Result<Cart>> GetUserCartAsync(string userId);
        Task<Result<CartItem>> GetUserCartItemAsync(string userId, int cartItemId);
        Task<Result<CartItem>> GetUserCartItemByProductIdAsync(string userId, int productId);
        Task<Result<List<CartItem>>> GetUserCartItemsAsync(string userId);
        Task<Result<bool>> UpdateCartItemAsync(CartItem cartItem);
    }
}