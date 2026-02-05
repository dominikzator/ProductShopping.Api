using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts
{
    public interface ICartItemsService
    {
        Task<Result<GetCartItemDto>> AddCartItemToCartAsync(CreateCartItemDto createCartItemDto);
        Task<Result> DeleteCartItemFromCartAsync(RemoveCartItemDto removeCartItemDto);
        Task<Result<GetCartItemDto>> GetCartItemAsync(int id);
        Task<Result<PagedResult<GetCartItemDto>>> GetCartItemsAsync(PaginationParameters paginationParameters);
    }
}