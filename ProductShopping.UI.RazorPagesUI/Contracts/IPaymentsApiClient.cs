using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;

namespace ProductShopping.UI.RazorPagesUI.Contracts
{
    public interface IPaymentsApiClient
    {
        Task<OrderDto> CreatePaymentSessionAsync(string accessToken, PaymentRequestDto request, CancellationToken cancellationToken = default);
    }
}