using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItems;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Models.Paging;

namespace ProductShopping.UI.RazorPagesUI.Contracts
{
    public interface ICartsApiClient
    {
        Task<CartItemDto?> AddCartItemAsync(string jwtToken, AddCartItemCommand command, CancellationToken ct = default);
        Task ClearCartAsync(string jwtToken, RemoveCartItemsCommand command, CancellationToken ct = default);
        Task<CartItemDto?> GetCartItemAsync(string jwtToken, int id, CancellationToken ct = default);
        Task<PagedResult<CartItemDto>?> GetCartItemsAsync(string jwtToken, PaginationParameters paginationParameters, CancellationToken ct = default);
        Task RemoveCartItemAsync(string jwtToken, RemoveCartItemCommand command, CancellationToken ct = default);
    }
}