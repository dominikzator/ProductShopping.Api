using ProductShopping.Api.Contracts;
using ProductShopping.Api.Controllers;
using Stripe.Checkout;

namespace ProductShopping.Api.Services;

public class PaymentsService : IPaymentsService
{
    public async Task CreatePaymentSession(CheckoutRequest checkoutRequest)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{checkoutRequest.Domain}/api/payments/success?session_id={{CHECKOUT_SESSION_ID}}", // Twoja strona sukcesu
            CancelUrl = $"{checkoutRequest.Domain}/api/payments/cancel", // Strona anulowania
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd", // Lub "pln"
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = checkoutRequest.ProductName ?? "Produkt",
                        Description = checkoutRequest.Description,
                    },
                    UnitAmount = (long)(checkoutRequest.Amount * 100), // np. 20.00 -> 2000
                },
                Quantity = 1,
            }
        },
            Mode = "payment",
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
    }
}
