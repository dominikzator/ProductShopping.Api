using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Features.Order.Commands.UpdateOrder;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.UI.Shared.Contracts;

namespace ProductShopping.UI.Shared.Clients;

public class OrdersApiClient(HttpClient httpClient) : IOrdersApiClient
{
    public async Task<PagedResult<OrderDto>> GetOrdersAsync(
        string accessToken,
        PaginationParameters paginationParameters,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            QueryHelpers.AddQueryString("api/orders", new Dictionary<string, string?>
            {
                ["PageNumber"] = paginationParameters.PageNumber.ToString(),
                ["PageSize"] = paginationParameters.PageSize.ToString()
            }));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PagedResult<OrderDto>>(cancellationToken: ct)
               ?? new PagedResult<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderAsync(string accessToken, int orderId, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/orders/{orderId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: ct);
    }

    public async Task<OrderDto?> CreateOrderAsync(string accessToken, CreateOrderCommand command, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/orders")
        {
            Content = JsonContent.Create(command)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: ct);
    }

    public async Task UpdateOrderAsync(string accessToken, int id, UpdateOrderCommand command, CancellationToken ct)
    {
        command.Id = id;

        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/orders/{id}")
        {
            Content = JsonContent.Create(command)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteOrderAsync(string accessToken, int orderId, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/orders/{orderId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}