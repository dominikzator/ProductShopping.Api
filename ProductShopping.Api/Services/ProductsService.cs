using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs;
using ProductShopping.Api.Extensions;
using ProductShopping.Api.Models.Filtering;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Services;

public class ProductsService(ProductShoppingDbContext context, IMapper mapper) : IProductsService
{
    public async Task<Result<PagedResult<GetProductsDto>>> GetCountriesAsync(PaginationParameters paginationParameters, ProductFilterParameters filters)
    {
        var query = context.Products.AsQueryable();

        if (filters.MinRating.HasValue)
        {
            query = query.Where(p => p.Rating >= filters.MinRating);
        }
        if (filters.MaxRating.HasValue)
        {
            query = query.Where(p => p.Rating <= filters.MaxRating);
        }
        if (filters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filters.MinPrice);
        }
        if (filters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filters.MaxPrice);
        }
        if(!string.IsNullOrWhiteSpace(filters.CategoryName))
        {
            query = query.Where(p => p.Category.Name.Contains(filters.CategoryName));
        }

        // generic search param
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim();
            query = query.Where(h => EF.Functions.Like(h.Name, $"%{search}%"));
        }

        query = filters.SortBy?.ToLower() switch
        {
            "name" => (bool)filters.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "rating" => (bool)filters.SortDescending ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating),
            "category" => (bool)filters.SortDescending ? query.OrderByDescending(p => p.Category.Name) : query.OrderBy(p => p.Category.Name),
            "price" => (bool)filters.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        var countries = await query
            .Include(q => q.Category)
            .AsNoTracking()
            .ProjectTo<GetProductsDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetProductsDto>>.Success(countries);
    }
}
