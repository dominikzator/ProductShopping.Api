using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using ProductShopping.UI.RazorPagesUI.Contracts;
using ProductShopping.UI.RazorPagesUI.DTOs.Products;
using System;
using System.Globalization;

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
    }

    public async Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(
            ProductsQueryDto query,
            CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>();

        queryParams["pageNumber"] = query.PageNumber.ToString();
        queryParams["pageSize"] = query.PageSize.ToString();

        if (!string.IsNullOrWhiteSpace(query.Search))
            queryParams["search"] = query.Search;

        if (!string.IsNullOrWhiteSpace(query.CategoryName))
            queryParams["categoryName"] = query.CategoryName;

        if (query.MinPrice.HasValue)
            queryParams["minPrice"] = query.MinPrice.Value.ToString(CultureInfo.InvariantCulture);

        if (query.MaxPrice.HasValue)
            queryParams["maxPrice"] = query.MaxPrice.Value.ToString(CultureInfo.InvariantCulture);

        if (query.MinRating.HasValue)
            queryParams["minRating"] = query.MinRating.Value.ToString(CultureInfo.InvariantCulture);

        if (query.MaxRating.HasValue)
            queryParams["maxRating"] = query.MaxRating.Value.ToString(CultureInfo.InvariantCulture);

        if (!string.IsNullOrWhiteSpace(query.SortBy))
            queryParams["sortBy"] = query.SortBy;

        queryParams["sortDescending"] = query.SortDescending.ToString().ToLowerInvariant();

        var url = QueryHelpers.AddQueryString("api/products", queryParams);

        var result = await _httpClient.GetFromJsonAsync<PagedResultDto<ProductListItemDto>>(url, ct);

        return result ?? new PagedResultDto<ProductListItemDto>();
    }

    public async Task<List<string>> GetCategoryNamesAsync(CancellationToken ct = default)
    {
        Console.WriteLine("GetCategoryNamesAsync");
        var result = await _httpClient.GetFromJsonAsync<List<string>>("api/products/categories", ct);
        return result ?? new List<string>();
    }
}

