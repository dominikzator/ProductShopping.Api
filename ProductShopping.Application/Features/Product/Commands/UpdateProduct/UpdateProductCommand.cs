using MediatR;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommand : CreateProductCommand, IRequest<Result>
{
    public int Id { get; set; }
}
