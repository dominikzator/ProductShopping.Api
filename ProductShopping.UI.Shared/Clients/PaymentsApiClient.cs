using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.UI.Shared.Contracts;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProductShopping.UI.Shared.Clients;

public class PaymentsApiClient : IPaymentsApiClient
{
    private readonly HttpClient _httpClient;

    public PaymentsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OrderDto> CreatePaymentSessionAsync(
        string accessToken,
        PaymentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/payments/session")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Payments API failed. Status={(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException("Payment session response was empty.");
        }

        return result;
    }
}