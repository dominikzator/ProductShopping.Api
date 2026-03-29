using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Queries.GetOrderDetails;

public class GetOrderDetailQueryHandler(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<GetOrderDetailQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();
        var order = await ordersRepository.GetUserOrderAsync(userId, request.Id.ToString());

        if (order.Value is null)
        {
            return Result<OrderDto>.Failure(new Error(ErrorCodes.NotFound, $"Order '{request.Id}' was not found."));
        }

        var outputDto = mapper.Map<OrderDto>(order.Value);

        return Result<OrderDto>.Success(outputDto);
    }
}