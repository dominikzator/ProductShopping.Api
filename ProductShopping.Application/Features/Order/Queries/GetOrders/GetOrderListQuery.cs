using MediatR;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Queries.GetOrders;

public class GetOrderListQuery : IRequest<Result<PagedResult<GetOrderDto>>>
{
    public PaginationParameters? PaginationParameters { get; set; }
}
