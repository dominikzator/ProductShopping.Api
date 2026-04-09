using ProductShopping.UI.RazorPagesUI.Clients;
using ProductShopping.UI.RazorPagesUI.DTOs.Products;

namespace ProductShopping.UI.RazorPagesUI.Contracts;

public interface IProductsApiClient
{
    Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(CancellationToken ct = default);
}
