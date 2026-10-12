using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.UI.Shared.DTOs.Payments;

namespace ProductShopping.UI.Shared.Contracts
{
    public interface IPaymentsApiClient
    {
        Task<OrderDto> CreatePaymentSessionAsync(string accessToken, PaymentRequestDto request, CancellationToken cancellationToken = default);
        Task<PaymentSessionStatusDto?> GetPaymentSessionStatusAsync(string sessionId, CancellationToken cancellationToken = default);
    }
}