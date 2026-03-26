using AutoMapper;
using ProductShopping.Application.Exceptions;
using MediatR;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductsRepository productsRepository, IMapper mapper) : IRequestHandler<CreateProductCommand, Result<GetProductDto>>
{
    public async Task<Result<GetProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator(productsRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new BadRequestException("Invalid Leave Allocation Request", validationResult);

        var category = await productsRepository.GetCategoryFromNameAsync(request.CategoryName);

        var product = mapper.Map<Domain.Models.Product>(request);
        product.CategoryId = category.Id;

        await productsRepository.CreateAsync(product);

        var outputDto = mapper.Map<GetProductDto>(product);

        return Result<GetProductDto>.Success(outputDto);
    }
}