using ProductShopping.Application.DTOs.Auth;
using ProductShopping.UI.Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProductShopping.UI.Shared.Clients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task LoginForBlazorAsync(LoginUserDto dto, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/blazor-auth/login", dto, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException($"Blazor login failed. Status={(int)response.StatusCode}, Details={details}");
    }

    public async Task LogoutForBlazorAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync("api/blazor-auth/logout", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public List<string> Errors { get; set; } = new();
}