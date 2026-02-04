using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs;
using ProductShopping.Api.Extensions;
using ProductShopping.Api.Models;
using ProductShopping.Api.Models.Filtering;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Services;

public class ProductsService(ProductShoppingDbContext context, IMapper mapper) : IProductsService
{
    public async Task<Result<PagedResult<GetProductDto>>> GetProductsAsync(PaginationParameters paginationParameters, ProductFilterParameters filters)
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
            .ProjectTo<GetProductDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetProductDto>>.Success(countries);
    }

    public async Task<Result<GetProductDto>> GetProductAsync(int id)
    {
        var product = await context.Products
            .Where(h => h.Id == id)
            .ProjectTo<GetProductDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return Result<GetProductDto>.Failure(new Error(ErrorCodes.NotFound, $"Product '{id}' was not found."));
        }

        return Result<GetProductDto>.Success(product);
    }

    public async Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto)
    {
        var category = await context.ProductCategories.FirstOrDefaultAsync(p => p.Name == productDto.CategoryName);

        if (category == null)
        {
            return Result<GetProductDto>.Failure(new Error(ErrorCodes.NotFound, $"Category '{productDto.CategoryName}' does not exist."));
        }

        var duplicate = await context.Products.AnyAsync(p => p.Name.ToLower().Trim() == productDto.Name.ToLower().Trim());

        if(duplicate)
        {
            return Result<GetProductDto>.Failure(new Error(ErrorCodes.Conflict, $"A Product with name: '{productDto.Name}' already exists."));
        }

        var product = mapper.Map<Product>(productDto);
        product.CategoryId = category.Id;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var outputDto = mapper.Map<GetProductDto>(product);

        return Result<GetProductDto>.Success(outputDto);
    }

    public async Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        if (id != productDto.Id)
        {
            return Result.BadRequest(new Error(ErrorCodes.Validation, "Id route value does not match payload Id."));
        }

        var product = await context.Products.FindAsync(id);
        if(product == null)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Product '{id}' was not found."));
        }

        var category = await context.ProductCategories.FirstOrDefaultAsync(p => p.Name == productDto.CategoryName);

        if (category == null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Category '{productDto.CategoryName}' does not exist."));
        }

        mapper.Map(productDto, product);
        context.Products.Update(product);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteProductAsync(int id)
    {
        var affected = await context.Products
            .Where(q => q.Id == id)
            .ExecuteDeleteAsync();

        if (affected == 0)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Product '{id}' was not found."));
        }

        return Result.Success();
    }
}
