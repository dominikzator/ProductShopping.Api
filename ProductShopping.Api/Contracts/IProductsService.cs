using ProductShopping.Api.DTOs;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IProductsService
    {
        Task<Result<IEnumerable<GetProductsDto>>> GetCountriesAsync();
    }
}