using ProductShopping.Application.DTOs.Auth;
using ProductShopping.UI.RazorPagesUI.Contracts;

namespace ProductShopping.UI.RazorPagesUI.Clients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Login(LoginUserDto loginUserDto, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginUserDto, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(ct);

        return result ?? string.Empty;
    }

    public async Task<RegisteredUserDto> Register(RegisterUserDto registerUserDto, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerUserDto, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<RegisteredUserDto>(cancellationToken: ct);

        return result!;
    }
}