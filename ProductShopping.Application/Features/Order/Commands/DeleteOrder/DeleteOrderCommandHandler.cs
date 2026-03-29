using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Commands.DeleteOrder;

public class DeleteOrderCommandHandler(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<DeleteOrderCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();

        var orderDto = await ordersRepository.GetUserOrderAsync(userId, request.Id.ToString());

        if (orderDto.Value is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order '{request.Id}' was not found."));
        }

        var order = mapper.Map<Domain.Models.Order>(orderDto);

        await ordersRepository.RemoveOrderItemsAsync(order.OrderItems);
        await ordersRepository.DeleteAsync(order);

        return Result.Success();
    }
}
