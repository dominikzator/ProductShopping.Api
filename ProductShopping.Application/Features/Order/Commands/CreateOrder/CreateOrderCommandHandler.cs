using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Features.Order.Commands.CreateOrder;

public class CreateOrderCommandHandler(IOrdersRepository ordersRepository, ICartsRepository cartsRepository, IUsersService usersService, 
    IPaymentsService paymentsService, IConfiguration config, IMapper mapper, IAppLogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userEmail = usersService.GetUserEmail();

        var validator = new CreateOrderCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
            throw new ValidationFailedException("Validation failed for Creating an Order");

        var userId = usersService.GetUserId();

        logger.LogInformation($"userId: {userId}");

        var userCart = await cartsRepository.GetUserCartAsync(userId);

        if (userCart.Value is null)
        {
            return Result<OrderDto>.Failure(new Error(ErrorCodes.Failure,
                $"User '{userEmail}'" +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        if (userCart.Value.CartItems.Count == 0)
        {
            return Result<OrderDto>.Failure(new Error(ErrorCodes.Failure, $"User Cart is empty"));
        }

        var orderNumber = GenerateOrderNumber();

        var order = new Domain.Models.Order
        {
            Address = request.Address,
            CustomerId = userId,
            OrderNumber = orderNumber
        };

        await ordersRepository.CreateAsync(order);

        var createdOrder = await ordersRepository.GetUserOrderByOrderNumberAsync(userId, orderNumber);

        foreach (var item in userCart.Value.CartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = createdOrder.Value!.Id,
                CustomerId = userId,
                ProductId = int.Parse(item.ProductId),
                ProductName = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.OverallPrice
            };

            await ordersRepository.AddOrderItemAsync(orderItem);
        }
        await cartsRepository.ClearCartAsync(userId);

        var orderItems = await ordersRepository.GetUserOrderItemsByOrderIdAsync(userId, createdOrder.Value!.Id.ToString());

        string domainName = config["Constants:DomainName"]!;

        var outputDto = await paymentsService.CreatePaymentSessionAsync(new DTOs.Payment.PaymentRequestDto
        {
            OrderId = createdOrder.Value.Id,
            Domain = domainName,
            OrderNumber = createdOrder.Value.OrderNumber,
            Items = mapper.Map<List<OrderItemDto>>(orderItems.Value),
            TotalPrice = ordersRepository.GetOrderItemsTotalPrice(orderItems.Value!).Value,
            UserEmail = userEmail
        });

        return Result<OrderDto>.Success(outputDto);
    }

    private string GenerateOrderNumber()
    {
        return $"ORDER-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
