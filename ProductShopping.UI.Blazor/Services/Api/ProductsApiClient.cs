using ProductShopping.UI.Blazor.Contracts;
using ProductShopping.UI.Blazor.DTOs.Products;

namespace ProductShopping.UI.Blazor.Services.Api
{
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

    public sealed class ProductsApiClient : IProductsApiClient
    {
        public PagedResultDto<ProductListItemDto> Products { get; private set; }

        private readonly HttpClient _httpClient;

        public ProductsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResultDto<ProductListItemDto>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            Products = await _httpClient.GetFromJsonAsync<PagedResultDto<ProductListItemDto>>(
                "api/products",
                cancellationToken);

            return Products ?? new PagedResultDto<ProductListItemDto> { 
                
            };
        }
    }
}
