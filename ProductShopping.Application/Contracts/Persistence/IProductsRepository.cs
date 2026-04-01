using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IProductsRepository : IGenericRepository<Product>
    {
        ProductCategoryDto? GetCategoryFromName(string categoryName);
        Task<(List<ProductDto> products, int TotalCount, int TotalPages)> GetFilteredRawPagedAsync(ProductFilterParameters filters, PaginationParameters paginationParameters);
        Task<ProductDto> GetProductDtoByNameAsync(string name);
        Task<Result<bool>> ValidateProductAsync(CreateProductCommand productDto);
    }
}