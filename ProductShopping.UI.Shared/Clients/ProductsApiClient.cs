using Microsoft.AspNetCore.WebUtilities;
using ProductShopping.UI.Shared.Contracts;
using ProductShopping.UI.Shared.DTOs.Products;
using ProductShopping.UI.Shared.Pagination;
using System.Net.Http.Json;

namespace ProductShopping.UI.Shared.Clients;

public class ProductsApiClient : IProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
        ProductsQueryDto query,
        CancellationToken ct = default)
    {
        query ??= new ProductsQueryDto();

        var queryParams = new Dictionary<string, string?>()
        {
            ["PageNumber"] = query.PageNumber.ToString(),
            ["PageSize"] = query.PageSize.ToString(),
            ["Search"] = query.Search,
            ["CategoryName"] = query.CategoryName,
            ["MinPrice"] = query.MinPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["MaxPrice"] = query.MaxPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["MinRating"] = query.MinRating?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["MaxRating"] = query.MaxRating?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["SortBy"] = query.SortBy,
            ["SortDescending"] = query.SortDescending.ToString().ToLowerInvariant()
        };

        var filteredQueryParams = queryParams
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .ToDictionary(x => x.Key, x => x.Value!);

        var url = QueryHelpers.AddQueryString("api/products", filteredQueryParams);

        var result = await _httpClient.GetFromJsonAsync<PagedResultDto<ProductListItemDto>>(url, ct);

        return result ?? new PagedResultDto<ProductListItemDto>();
    }

    public async Task<List<string>> GetCategoryNamesAsync(CancellationToken ct = default)
    {
        var result = await _httpClient.GetFromJsonAsync<List<string>>("api/products/categories", ct);
        return result ?? new List<string>();
    }
}