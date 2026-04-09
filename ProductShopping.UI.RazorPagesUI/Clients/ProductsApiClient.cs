using ProductShopping.UI.RazorPagesUI.Contracts;
using ProductShopping.UI.RazorPagesUI.DTOs.Products;

namespace ProductShopping.UI.RazorPagesUI.Clients;

public sealed class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public PaginationMetadataDto Metadata { get; set; } = new();
}

public sealed class PaginationMetadataDto
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}

public class ProductsApiClient : IProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        var baseAddress = _httpClient.BaseAddress;
    }

    public async Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(CancellationToken ct = default)
    {
        var result = await _httpClient.GetFromJsonAsync<PagedResultDto<ProductListItemDto>>("api/products", ct);
        return result ?? new PagedResultDto<ProductListItemDto>();
    }
}

