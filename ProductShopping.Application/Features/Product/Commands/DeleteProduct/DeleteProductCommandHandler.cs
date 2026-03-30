using MediatR;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IProductsRepository productsRepository) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            throw new NotFoundException($"Product '{request.Id}' was not found.");
        }

        await productsRepository.DeleteAsync(product);

        return Result.Success();
    }
}