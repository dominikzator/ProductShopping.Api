using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Services;

public class ProductsService(ProductShoppingDbContext context, IMapper mapper) : IProductsService
{
    public async Task<Result<IEnumerable<GetProductsDto>>> GetCountriesAsync()
    {
        var query = context.Products.AsQueryable();

        var countries = await query
            .AsNoTracking()
            .ProjectTo<GetProductsDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetProductsDto>>.Success(countries);
    }
}
