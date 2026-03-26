using MediatR;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Queries.GetOrderDetails;

public class GetOrderDetailQuery : IRequest<Result<GetOrderDto>>
{
    public int Id { get; set; }
}
