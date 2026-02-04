using ProductShopping.Api.DTOs.Product;
using ProductShopping.Api.Models.Filtering;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts;

public interface IProductsService
{
    Task<Result<PagedResult<GetProductDto>>> GetProductsAsync(PaginationParameters paginationParameters, ProductFilterParameters filters);
    Task<Result<GetProductDto>> GetProductAsync(int id);
    Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto);
    Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto);
    Task<Result> DeleteProductAsync(int id);
}