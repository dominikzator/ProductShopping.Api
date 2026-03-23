using ProductShopping.Application.DTOs.CartItem;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Contracts
{
    public interface ICartItemsService
    {
        Task<Result<GetCartItemDto>> AddCartItemToCartAsync(CreateCartItemDto createCartItemDto);
        Task<Result> ClearCartAsync();
        Task<Result> DeleteCartItemFromCartAsync(RemoveCartItemDto removeCartItemDto);
        Task<Result<GetCartItemDto>> GetCartItemAsync(int id);
        Task<Result<PagedResult<GetCartItemDto>>> GetCartItemsAsync(PaginationParameters paginationParameters);
    }
}