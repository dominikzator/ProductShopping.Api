using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Commands.DeleteOrder;

public class DeleteOrderCommandHandler(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<DeleteOrderCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = usersService.GetUserId();

        var orderDto = await ordersRepository.GetUserOrderDtoAsync(userId, request.Id);

        if (orderDto.Value is null)
        {
            throw new NotFoundException($"Order '{request.Id}' was not found.");
        }

        var order = mapper.Map<Domain.Models.Order>(orderDto);
        var orderItems = mapper.Map<List<Domain.Models.OrderItem>>(orderDto.Value.OrderItems);

        await ordersRepository.RemoveOrderItemsAsync(orderItems);
        await ordersRepository.DeleteAsync(order);

        return Result.Success();
    }
}
