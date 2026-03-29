using AutoMapper;
using ProductShopping.Application.Exceptions;
using MediatR;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Queries.GetProductDetails;

public class GetProductDetailQueryHandler(IProductsRepository productsRepository, IMapper mapper) : IRequestHandler<GetProductDetailQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.Id);

        if (product is null)
        {
            throw new NotFoundException($"Product with id: {request.Id} has not been found");
        }

        var productDto = mapper.Map<ProductDto>(product);

        return Result<ProductDto>.Success(productDto);
    }
}
