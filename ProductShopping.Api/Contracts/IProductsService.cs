using ProductShopping.Api.DTOs;
using ProductShopping.Api.Models.Filtering;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IProductsService
    {
        Task<Result<PagedResult<GetProductsDto>>> GetCountriesAsync(PaginationParameters paginationParameters, ProductFilterParameters filters);
    }
}