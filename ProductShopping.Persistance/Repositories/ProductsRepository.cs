using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
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

    public List<Product> Products;

    public ProductsRepository(ProductShoppingDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
        Products = _context.Products.ToList();
    }

    public async Task<Result<bool>> ValidateProductAsync(CreateProductCommand productDto)
    {
        var category = _context.GetProductCategoriesAsNoTracking().Result.FirstOrDefault(p => p.Name == productDto.CategoryName);

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

    public async Task<(List<ProductDto> products, int TotalCount, int TotalPages)> GetFilteredRawPagedAsync(ProductFilterParameters filters, PaginationParameters paginationParameters)
    {
        Console.WriteLine("filters.CategoryName: " + filters.CategoryName);
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

        var finalItemsDtos = _mapper.Map<List<ProductDto>>(finalItems);

        var totalCount = queryableProducts.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParameters.PageSize);

        return (finalItemsDtos, totalCount, totalPages);
    }

    public async Task<Product?> GetByIdAsync(int id, bool tracking = false)
    {
        var product = tracking ? await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id)
            : await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return product;
    }

    public ProductCategoryDto? GetCategoryFromName(string categoryName)
    {
        var productCategory = _context.GetProductCategoriesAsNoTracking().Result.FirstOrDefault(p => p.Name == categoryName);
        var productCategoryDto = _mapper.Map<ProductCategoryDto>(productCategory);

        return productCategoryDto;
    }

    public async Task<ProductDto?> GetProductDtoByNameAsync(string name)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Name == name);
        var productDto = _mapper.Map<ProductDto>(product);

        return productDto;
    }

    public async Task<Product?> GetProductByNameAsync(string name)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);

        return product;
    }

    public async Task<List<string>> GetCategoryNamesAsync() => _context.ProductCategories.Select(p => p.Name).ToList();
}