using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Queries.GetOrders;

public class GetOrderListQueryHandler(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetOrderListQuery, Result<PagedResult<GetOrderDto>>>
{
    public async Task<Result<PagedResult<GetOrderDto>>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();

        var userOrders = await ordersRepository.GetUserOrdersAsync(userId);

        var dtos = mapper.Map<List<GetOrderDto>>(userOrders);

        var pagedResult = new PagedResult<GetOrderDto>
        {
            Data = PagedResult<GetOrderDto>.GetData(dtos, request.PaginationParameters!),
            Metadata = PagedResult<GetOrderDto>.GetPaginationMetadata(dtos, request.PaginationParameters!)
        };

        return Result<PagedResult<GetOrderDto>>.Success(pagedResult);
    }
}
