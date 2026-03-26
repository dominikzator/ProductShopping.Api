using AutoMapper;
using MediatR;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductsRepository productsRepository, IMapper mapper) : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator(productsRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new BadRequestException("Invalid Leave Allocation Request", validationResult);

        var product = await productsRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new NotFoundException($"Product '{request.Id}' was not found.");
        }

        var category = await productsRepository.GetCategoryFromNameAsync(request.CategoryName);

        if (category == null)
        {
            throw new NotFoundException($"Category Name: {request.CategoryName} has not been found");
        }

        mapper.Map(request, product);
        await productsRepository.UpdateAsync(product);

        return Result.Success();
    }
}
