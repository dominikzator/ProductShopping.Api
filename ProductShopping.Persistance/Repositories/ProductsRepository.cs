using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Product;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Persistence.Repositories;

public class ProductsRepository : GenericRepository<Product>, IProductsRepository
{
    private readonly ProductShoppingDbContext _context;
    private readonly IMapper _mapper;

    public ProductsRepository(ProductShoppingDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<bool>> ValidateProductAsync(CreateProductDto productDto)
    {
        var category = await _context.ProductCategories.FirstOrDefaultAsync(p => p.Name == productDto.CategoryName);

        if (category == null)
        {
            return Result<bool>.Failure(new Error(ErrorCodes.NotFound, $"Category '{productDto.CategoryName}' does not exist."));
        }

        var duplicate = await _context.Products.AnyAsync(p => p.Name.ToLower().Trim() == productDto.Name.ToLower().Trim());

        if (duplicate)
        {
            return Result<bool>.Failure(new Error(ErrorCodes.Conflict, $"A Product with name: '{productDto.Name}' already exists."));
        }

        return Result<bool>.Success(true);
    }

    public async Task<(List<Product> products, int TotalCount, int TotalPages)> GetFilteredRawPagedAsync(ProductFilterParameters filters, PaginationParameters paginationParameters)
    {
        var products = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim();
            products = products.Where(h => EF.Functions.Like(h.Name, $"%{search}%"));
        }

        products = filters.SortBy?.ToLower() switch
        {
            "name" => (bool)filters.SortDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
            "rating" => (bool)filters.SortDescending ? products.OrderByDescending(p => p.Rating) : products.OrderBy(p => p.Rating),
            "category" => (bool)filters.SortDescending ? products.OrderByDescending(p => p.Category.Name) : products.OrderBy(p => p.Category.Name),
            "price" => (bool)filters.SortDescending ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
            _ => products.OrderBy(p => p.Name)
        };

        if (filters.MinRating.HasValue)
        {
            products = products.Where(p => p.Rating >= filters.MinRating);
        }
        if (filters.MaxRating.HasValue)
        {
            products = products.Where(p => p.Rating <= filters.MaxRating);
        }
        if (filters.MinPrice.HasValue)
        {
            products = products.Where(p => p.Price >= filters.MinPrice);
        }
        if (filters.MaxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= filters.MaxPrice);
        }
        if (!string.IsNullOrWhiteSpace(filters.CategoryName))
        {
            products = products.Where(p => p.Category.Name.Contains(filters.CategoryName));
        }

        var queryableProducts = products
            .Include(q => q.Category).AsNoTracking();

        var finalItems = await queryableProducts
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        var totalCount = queryableProducts.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParameters.PageSize);

        return (finalItems, totalCount, totalPages);
    }

    public async Task<ProductCategory?> GetCategoryFromNameAsync(string categoryName) => await _context.ProductCategories.FirstOrDefaultAsync(p => p.Name == categoryName);

    public async Task<Product?> GetProductByNameAsync(string name) => await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
}