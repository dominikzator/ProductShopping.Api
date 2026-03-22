using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Payment;
using ProductShopping.Api.Results;
using ProductShopping.Domain.Enums;
using ProductShopping.Persistence.DatabaseContext;
using Stripe.Checkout;

namespace ProductShopping.Api.Services;

public class PaymentsService(ProductShoppingDbContext context) : IPaymentsService
{
    public async Task<Session> CreatePaymentSessionAsync(PaymentRequestDto paymentRequest)
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

        return await service.CreateAsync(options);
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
