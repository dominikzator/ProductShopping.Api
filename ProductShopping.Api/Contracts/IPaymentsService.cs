using ProductShopping.Api.Controllers;

namespace ProductShopping.Api.Contracts
{
    public interface IPaymentsService
    {
        Task CreatePaymentSession(CheckoutRequest checkoutRequest);
    }
}