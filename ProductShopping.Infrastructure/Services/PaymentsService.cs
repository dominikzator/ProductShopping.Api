using AutoMapper;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using Stripe.Checkout;

namespace ProductShopping.Application.Services;

public class PaymentsService(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper, IAppLogger<PaymentsService> logger) : IPaymentsService
{
    public async Task<OrderDto> CreatePaymentSessionAsync(PaymentRequestDto paymentRequest)
    {
        List<SessionLineItemOptions> lineItems = new();
        foreach (var item in paymentRequest.Items)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "pln",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName,
                        Description = $"{item.Quantity}x {item.ProductName}",
                    },
                    UnitAmount = (long)(item.UnitPrice * 100),
                },
                Quantity = item.Quantity
            });
        }
        var userId = usersService.GetUserId();

        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{paymentRequest.Domain}/api/payments/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{paymentRequest.Domain}/api/payments/cancel",
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            ClientReferenceId = paymentRequest.OrderId.ToString(),
            Mode = "payment",
            Metadata = new Dictionary<string, string>
            {
                {"userEmail", paymentRequest.UserEmail},
                {"userId", userId}
            }
        };

        var service = new SessionService();

        var session = await service.CreateAsync(options);


        var order = await ordersRepository.GetUserOrderAsync(userId, paymentRequest.OrderId.ToString());
        var orderItems = await ordersRepository.GetUserOrderItemsByOrderIdAsync(userId, paymentRequest.OrderId.ToString());

        var outputDto = mapper.Map<OrderDto>(order.Value);
        outputDto.PaymentUrl = session.Url;
        outputDto.OrderItems = mapper.Map<List<OrderItemDto>>(orderItems.Value);

        return outputDto;
    }

    public async Task<Result> SetOrderPayed(string userId, int orderId)
    {
        logger.LogInformation($"SetOrderPayed: orderId: {orderId}, userId: {userId}");

        var result = await ordersRepository.SetUserOrderPayedAsync(userId, orderId);
        if (!result.IsSuccess)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"{result.Errors[0]}"));
        }

        return Result.Success();
    }
}
