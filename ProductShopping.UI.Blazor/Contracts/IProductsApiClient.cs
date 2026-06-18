using ProductShopping.UI.Blazor.DTOs.Products;
using ProductShopping.UI.Blazor.Services.Api;

namespace ProductShopping.UI.Blazor.Contracts
{
    public interface IProductsApiClient
    {
        Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
