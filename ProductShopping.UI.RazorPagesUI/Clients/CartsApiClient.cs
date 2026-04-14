using System.Net.Http.Headers;
using System.Net.Http.Json;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItems;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItems;
using ProductShopping.Application.Models.Paging;
using ProductShopping.UI.RazorPagesUI.Contracts;

namespace ProductShopping.UI.RazorPagesUI.Clients;

public class CartsApiClient(HttpClient httpClient) : ICartsApiClient
{
    public async Task<PagedResult<CartItemDto>?> GetCartItemsAsync(
        string jwtToken,
        PaginationParameters paginationParameters,
        CancellationToken ct = default)
    {
        SetBearerToken(jwtToken);

        var query = $"api/cart?pageNumber={paginationParameters.PageNumber}&pageSize={paginationParameters.PageSize}";
        return await httpClient.GetFromJsonAsync<PagedResult<CartItemDto>>(query, ct);
    }

    public async Task<CartItemDto?> GetCartItemAsync(
        string jwtToken,
        int id,
        CancellationToken ct = default)
    {
        SetBearerToken(jwtToken);

        return await httpClient.GetFromJsonAsync<CartItemDto>($"api/cart/{id}", ct);
    }

    public async Task<CartItemDto?> AddCartItemAsync(
        string jwtToken,
        AddCartItemCommand command,
        CancellationToken ct = default)
    {
        SetBearerToken(jwtToken);

        var response = await httpClient.PostAsJsonAsync("api/cart", command, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CartItemDto>(cancellationToken: ct);
    }

    public async Task RemoveCartItemAsync(
        string jwtToken,
        RemoveCartItemCommand command,
        CancellationToken ct = default)
    {
        SetBearerToken(jwtToken);

        using var request = new HttpRequestMessage(HttpMethod.Delete, "api/cart")
        {
            Content = JsonContent.Create(command)
        };

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task ClearCartAsync(
        string jwtToken,
        RemoveCartItemsCommand command,
        CancellationToken ct = default)
    {
        SetBearerToken(jwtToken);

        using var request = new HttpRequestMessage(HttpMethod.Delete, "api/cart/clear")
        {
            Content = JsonContent.Create(command)
        };

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    private void SetBearerToken(string jwtToken)
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwtToken);
    }
}