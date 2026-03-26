using AutoMapper;
using MediatR;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IProductsRepository productsRepository, IMapper mapper) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Product '{request.Id}' was not found."));
        }

        await productsRepository.DeleteAsync(product);

        return Result.Success();
    }
}