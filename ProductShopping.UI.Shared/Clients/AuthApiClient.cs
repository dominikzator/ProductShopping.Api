using ProductShopping.Application.DTOs.Auth;
using ProductShopping.UI.Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProductShopping.UI.Shared.Clients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Login(LoginUserDto loginUserDto, CancellationToken ct)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginUserDto);

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Login request failed with status {(int)response.StatusCode}. Body: {body}");
        }

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (apiResponse is null)
        {
            throw new InvalidOperationException("Login API returned empty response.");
        }

        if (!apiResponse.IsSuccess || string.IsNullOrWhiteSpace(apiResponse.Value))
        {
            var errors = apiResponse.Errors is { Count: > 0 }
                ? string.Join("; ", apiResponse.Errors)
                : "Unknown login error.";

            throw new InvalidOperationException($"Login API did not return a valid token. {errors}");
        }

        return apiResponse.Value;
    }

    public async Task<RegisteredUserDto> Register(RegisterUserDto registerUserDto, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerUserDto, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<RegisteredUserDto>(cancellationToken: ct);

        return result!;
    }
}

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public List<string> Errors { get; set; } = new();
}