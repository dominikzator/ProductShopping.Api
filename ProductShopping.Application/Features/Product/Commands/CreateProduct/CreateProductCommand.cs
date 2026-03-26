using MediatR;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Results;
using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Result<GetProductDto>>
{
    [Required]
    public required string Name { get; set; }

    [Range(1.0, 5.0)]
    public double Rating { get; set; }

    [Range(0.01, 5000)]
    public decimal Price { get; set; }

    [Required]
    public string CategoryName { get; set; }
}
