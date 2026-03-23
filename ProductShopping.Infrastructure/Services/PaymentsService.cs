using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.DTOs.Order;
using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Enums;
using ProductShopping.Persistence.DatabaseContext;
using Stripe.Checkout;

namespace ProductShopping.Application.Services;

public class PaymentsService(ProductShoppingDbContext context, IMapper mapper) : IPaymentsService
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

        var outputDto = mapper.Map<GetOrderDto>(paymentRequest);
        outputDto.PaymentUrl = session.Url;
        outputDto.OrderItems = mapper.Map<List<GetOrderItemDto>>(orderItems);

        return outputDto;
    }

    public async Task<Result> SetOrderPayed(int orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if(order == null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order '{orderId}' was not found."));
        }
        order.OrderStatus = OrderStatus.Payed;
        context.Orders.Update(order);

        foreach (var orderItem in context.OrderItems.Where(o => o.OrderId == orderId))
        {
            orderItem.OrderStatus = OrderStatus.Payed;
            context.OrderItems.Update(orderItem);
        }

        await context.SaveChangesAsync();

        return Result.Success();
    }
}
