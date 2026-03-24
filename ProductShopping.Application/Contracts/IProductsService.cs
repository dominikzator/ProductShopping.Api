using ProductShopping.Application.DTOs.Product;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Contracts;

public interface IProductsService
{
    Task<Result<PagedResult<GetProductDto>>> GetProductsAsync(ProductFilterParameters filters, PaginationParameters paginationParameters);
    Task<Result<GetProductDto>> GetProductAsync(int id);
    Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto);
    Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto);
    Task<Result> DeleteProductAsync(int id);
}