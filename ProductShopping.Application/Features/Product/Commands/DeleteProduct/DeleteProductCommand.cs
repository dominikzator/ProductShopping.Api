using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Result>
{
    public int Id { get; set; }
}
