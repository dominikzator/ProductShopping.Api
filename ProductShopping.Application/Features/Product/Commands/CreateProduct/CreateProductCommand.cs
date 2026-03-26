using MediatR;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Results;
using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Result<GetProductDto>>
{
    public required string Name { get; set; }
    public double Rating { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
}
