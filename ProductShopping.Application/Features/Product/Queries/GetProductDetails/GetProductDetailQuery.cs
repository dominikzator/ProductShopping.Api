using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Product.Queries.GetProductDetails;

public class GetProductDetailQuery : IRequest<Result<GetProductDto>>
{
    public int Id { get; set; }
}
