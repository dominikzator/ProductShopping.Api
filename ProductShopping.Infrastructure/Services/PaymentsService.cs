using AutoMapper;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Order;
using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using Stripe.Checkout;

namespace ProductShopping.Application.Services;

public class PaymentsService(IOrdersRepository ordersRepository, IUsersService usersService, IMapper mapper) : IPaymentsService
{
    public async Task<GetOrderDto> CreatePaymentSessionAsync(PaymentRequestDto paymentRequest)
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
                {"userEmail", paymentRequest.UserEmail}
            }
        };

        var service = new SessionService();

        var session = await service.CreateAsync(options);

        var userId = usersService.GetUserId();

        var order = await ordersRepository.GetUserOrderAsync(userId, paymentRequest.OrderId.ToString());
        var orderItems = await ordersRepository.GetUserOrderItemsByOrderIdAsync(userId, paymentRequest.OrderId.ToString());

        var outputDto = mapper.Map<GetOrderDto>(order.Value);
        outputDto.PaymentUrl = session.Url;
        outputDto.OrderItems = mapper.Map<List<GetOrderItemDto>>(orderItems.Value);

        return outputDto;
    }

    public async Task<Result> SetOrderPayed(int orderId)
    {
        var userId = usersService.GetUserId();

        var order = ordersRepository.GetUserOrderAsync(userId, orderId.ToString());
        if(order == null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order '{orderId}' was not found."));
        }

        await ordersRepository.SetUserOrderPayedAsync(userId, orderId);

        return Result.Success();
    }
}
