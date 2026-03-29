using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Contracts
{
    public interface IPaymentsService
    {
        Task<OrderDto> CreatePaymentSessionAsync(PaymentRequestDto checkoutRequest);
        Task<Result> SetOrderPayed(string userId, int orderId);
    }
}