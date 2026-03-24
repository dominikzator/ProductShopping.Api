using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Product;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Services;

public class ProductsService(IProductsRepository productsRepository, IMapper mapper) : IProductsService
{
    public async Task<Result<PagedResult<GetProductDto>>> GetProductsAsync(ProductFilterParameters filters, PaginationParameters paginationParameters)
    {
        var filteredResult = await productsRepository.GetFilteredRawPagedAsync(filters, paginationParameters);

        var dtoProducts = filteredResult.products.AsQueryable().ProjectTo<GetProductDto>(mapper.ConfigurationProvider);

        var metadata = new PaginationMetadata
        {
            CurrentPage = paginationParameters.PageNumber,
            PageSize = paginationParameters.PageSize,
            TotalCount = filteredResult.TotalCount,
            TotalPages = filteredResult.TotalPages,
            HasNext = paginationParameters.PageNumber < filteredResult.TotalPages,
            HasPrevious = paginationParameters.PageNumber > 1
        };

        var pagedResult = new PagedResult<GetProductDto>
        {
            Data = dtoProducts,
            Metadata = metadata
        };

        return Result<PagedResult<GetProductDto>>.Success(pagedResult);
    }

    public async Task<Result<GetProductDto>> GetProductAsync(int id)
    {
        var product = await productsRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<GetProductDto>.Failure(new Error(ErrorCodes.NotFound, $"Product '{id}' was not found."));
        }

        var productDto = mapper.Map<GetProductDto>(product);

        return Result<GetProductDto>.Success(productDto);
    }

    public async Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto)
    {
        if(productsRepository.ValidateProductAsync(productDto).Result.Errors.Length > 0)
        {
            return Result<GetProductDto>.Failure(new Error(ErrorCodes.Validation, $"Validation failed for Creating a Product"));
        }

        var category = await productsRepository.GetCategoryFromNameAsync(productDto.CategoryName);

        if (category is null)
        {
            return Result<GetProductDto>.Failure(new Error(ErrorCodes.NotFound, $"Category Name: {productDto.CategoryName} has not been found"));
        }

        var product = mapper.Map<Product>(productDto);
        product.CategoryId = category.Id;

        await productsRepository.CreateAsync(product);

        var outputDto = mapper.Map<GetProductDto>(product);

        return Result<GetProductDto>.Success(outputDto);
    }

    public async Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        if (id != productDto.Id)
        {
            return Result.BadRequest(new Error(ErrorCodes.Validation, "Id route value does not match payload Id."));
        }

        var product = await productsRepository.GetByIdAsync(id);
        if(product == null)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Product '{id}' was not found."));
        }

        var category = await productsRepository.GetCategoryFromNameAsync(productDto.CategoryName);

        if (category == null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Category Name: {productDto.CategoryName} has not been found"));
        }

        mapper.Map(productDto, product);
        await productsRepository.UpdateAsync(product);

        return Result.Success();
    }

    public async Task<Result> DeleteProductAsync(int id)
    {
        var product = await productsRepository.GetByIdAsync(id);

        if (product == null)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Product '{id}' was not found."));
        }

        await productsRepository.DeleteAsync(product);

        return Result.Success();
    }
}
