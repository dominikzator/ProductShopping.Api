using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Queries.GetProducts;

public class GetProductListHandler(IProductsRepository productsRepository, IMapper mapper) : IRequestHandler<GetProductListQuery, Result<PagedResult<GetProductDto>>>
{
    public async Task<Result<PagedResult<GetProductDto>>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var filteredResult = await productsRepository.GetFilteredRawPagedAsync(request.ProductFilterParameters!, request.PaginationParameters!);

        var dtoProducts = filteredResult.products.AsQueryable().ProjectTo<GetProductDto>(mapper.ConfigurationProvider);

        var metadata = new PaginationMetadata
        {
            CurrentPage = request.PaginationParameters!.PageNumber,
            PageSize = request.PaginationParameters.PageSize,
            TotalCount = filteredResult.TotalCount,
            TotalPages = filteredResult.TotalPages,
            HasNext = request.PaginationParameters.PageNumber < filteredResult.TotalPages,
            HasPrevious = request.PaginationParameters.PageNumber > 1
        };

        var pagedResult = new PagedResult<GetProductDto>
        {
            Data = dtoProducts,
            Metadata = metadata
        };

        return Result<PagedResult<GetProductDto>>.Success(pagedResult);
    }
}
