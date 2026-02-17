using ProductShopping.Api.Controllers;
using Stripe.Checkout;

namespace ProductShopping.Api.Contracts
{
    public interface IPaymentsService
    {
        Task<Session> CreatePaymentSession(CheckoutRequest checkoutRequest);
    }
}