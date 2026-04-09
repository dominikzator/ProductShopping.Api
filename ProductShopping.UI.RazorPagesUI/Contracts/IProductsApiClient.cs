using ProductShopping.UI.RazorPagesUI.Clients;
using ProductShopping.UI.RazorPagesUI.DTOs.Products;

namespace ProductShopping.UI.RazorPagesUI.Contracts;

public interface IProductsApiClient
{
    Task<List<string>> GetCategoryNamesAsync(CancellationToken ct = default);
    Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
        ProductsQueryDto query,
        CancellationToken ct = default);
}
