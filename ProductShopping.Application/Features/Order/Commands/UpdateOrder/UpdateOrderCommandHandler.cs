using AutoMapper;
using MediatR;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Features.Order.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IRequestHandler<UpdateOrderCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var userEmail = usersService.GetUserEmail();

        var validator = new CreateOrderCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new ValidationFailedException("Validation failed for Updating an Order");

        var order = await ordersRepository.GetByIdAsync(request.Id);
        if (order == null)
        {
            throw new NotFoundException($"Order '{request.Id}' was not found.");
        }

        mapper.Map(request, order);
        await ordersRepository.UpdateAsync(order);

        return Result.Success();
    }
}
