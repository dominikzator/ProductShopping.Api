using ProductShopping.Api.DTOs.Payment;
using ProductShopping.Api.Results;
using Stripe.Checkout;

namespace ProductShopping.Api.Contracts
{
    public interface IPaymentsService
    {
        Task<Session> CreatePaymentSessionAsync(PaymentRequestDto checkoutRequest);
        Task<Result> SetOrderPayed(int orderId);
    }
}