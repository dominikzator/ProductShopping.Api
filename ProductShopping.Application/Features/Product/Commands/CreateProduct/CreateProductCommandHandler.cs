using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductsRepository productsRepository, IMapper mapper, IAppLogger<CreateProductCommandHandler> logger) : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateProductCommandHandler Handler");
        var validator = new CreateProductCommandValidator(productsRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new ValidationFailedException("Validation failed for Creating a Product");

        var category = productsRepository.GetCategoryFromName(request.CategoryName);

        var product = mapper.Map<Domain.Models.Product>(request);

        logger.LogInformation($"{product}");
        logger.LogInformation($"{product.Category}");
        product.CategoryId = category.Id;

        await productsRepository.CreateAsync(product);

        var outputDto = mapper.Map<ProductDto>(product);

        return Result<ProductDto>.Success(outputDto);
    }
}