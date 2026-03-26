using ProductShopping.Application.DTOs.Product;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IProductsRepository : IGenericRepository<Product>
    {
        Task<ProductCategory?> GetCategoryFromNameAsync(string categoryName);
        Task<(List<Product> products, int TotalCount, int TotalPages)> GetFilteredRawPagedAsync(ProductFilterParameters filters, PaginationParameters paginationParameters);
        Task<Product> GetProductByNameAsync(string name);
        Task<Result<bool>> ValidateProductAsync(CreateProductDto productDto);
    }
}