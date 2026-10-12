using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.UI.Blazor.Contracts;
using ProductShopping.UI.Shared.Contracts;

namespace ProductShopping.UI.Blazor.Services;

/// <summary>
/// Wspólna logika płatności zamówień: token, e-mail użytkownika i tworzenie
/// nowej sesji Stripe Checkout (Orders, OrderPayment/cancel).
/// </summary>
public sealed class OrderPaymentService(
    IOrdersApiClient ordersApiClient,
    IPaymentsApiClient paymentsApiClient,
    ITokenStore tokenStore,
    AuthenticationStateProvider authenticationStateProvider,
    NavigationManager navigationManager)
{
    /// <summary>Zwraca access token z ProtectedSessionStorage albo null, gdy użytkownik nie jest zalogowany.</summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        var rawTokenResponse = await tokenStore.GetAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(rawTokenResponse))
            return null;

        var tokenResult = JsonSerializer.Deserialize<AccessTokenResponse>(rawTokenResponse);

        if (tokenResult is null || !tokenResult.isSuccess || string.IsNullOrWhiteSpace(tokenResult.value))
            return null;

        return tokenResult.value;
    }

    /// <summary>
    /// Tworzy nową sesję płatności Stripe dla zamówienia i zwraca jej adres.
    /// Zwraca null, gdy zamówienie jest już opłacone.
    /// </summary>
    public async Task<string?> CreatePaymentUrlAsync(string accessToken, int orderId, CancellationToken ct = default)
    {
        // Świeże dane zamówienia (pozycje i status) przed utworzeniem sesji.
        var order = await ordersApiClient.GetOrderAsync(accessToken, orderId, ct)
            ?? throw new InvalidOperationException("The order could not be found.");

        if (IsPaid(order.OrderStatus))
            return null;

        var items = order.OrderItems ?? new List<OrderItemDto>();

        var result = await paymentsApiClient.CreatePaymentSessionAsync(accessToken, new PaymentRequestDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            UserEmail = await GetUserEmailAsync(),
            // API buduje z tego adresu SuccessUrl/CancelUrl płatności Stripe.
            Domain = navigationManager.BaseUri.TrimEnd('/'),
            Items = items,
            TotalPrice = items.Sum(x => x.TotalPrice)
        }, ct);

        if (string.IsNullOrWhiteSpace(result.PaymentUrl))
            throw new InvalidOperationException("The payment session was created, but the payment link is missing.");

        return result.PaymentUrl;
    }

    /// <summary>OrderStatus z API: Pending, Payed, Shipped, Delivered, Cancelled.</summary>
    public static bool IsPaid(string? status) => status?.Trim().ToLowerInvariant() is "payed" or "paid" or "shipped" or "delivered";

    private async Task<string> GetUserEmailAsync()
    {
        var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

        return user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("email")?.Value
            ?? user.Identity?.Name
            ?? string.Empty;
    }
}
