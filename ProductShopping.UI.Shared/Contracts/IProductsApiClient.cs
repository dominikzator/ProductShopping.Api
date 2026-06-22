using ProductShopping.UI.Shared.Pagination;
using ProductShopping.UI.Shared.DTOs.Products;

namespace ProductShopping.UI.Shared.Contracts;

public interface IProductsApiClient
{
    Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
        ProductsQueryDto query,
        CancellationToken ct = default);

    Task<List<string>> GetCategoryNamesAsync(CancellationToken ct = default);
}