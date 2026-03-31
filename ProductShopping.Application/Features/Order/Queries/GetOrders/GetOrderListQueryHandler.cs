using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Queries.GetOrders;

public class GetOrderListQueryHandler(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetOrderListQuery, Result<PagedResult<OrderDto>>>
{
    public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();

        var userOrders = await ordersRepository.GetUserOrdersAsync(userId);

        var pagedResult = new PagedResult<OrderDto>
        {
            Data = PagedResult<OrderDto>.GetData(userOrders, request.PaginationParameters!),
            Metadata = PagedResult<OrderDto>.GetPaginationMetadata(userOrders, request.PaginationParameters!)
        };

        return Result<PagedResult<OrderDto>>.Success(pagedResult);
    }
}
