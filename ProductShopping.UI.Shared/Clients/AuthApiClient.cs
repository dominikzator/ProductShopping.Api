using ProductShopping.Application.DTOs.Auth;
using ProductShopping.UI.Shared.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace ProductShopping.UI.Shared.Services;

public sealed class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<string> LoginAsync(
        LoginUserDto dto,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            dto,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Nieprawidłowy email lub hasło.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Login request failed. Status: {(int)response.StatusCode}. Response: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("API returned empty login response.");
        }

        if (content.StartsWith("\"") && content.EndsWith("\""))
        {
            content = System.Text.Json.JsonSerializer.Deserialize<string>(content)
                      ?? throw new InvalidOperationException("API returned invalid login token.");
        }

        return content;
    }

    public async Task<RegisteredUserDto> RegisterAsync(
        RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/register",
            dto,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var validationContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Registration validation failed: {validationContent}");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Register request failed. Status: {(int)response.StatusCode}. Response: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<RegisteredUserDto>(
            cancellationToken: cancellationToken);

        return result ?? throw new InvalidOperationException("API returned empty register response.");
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}