using Microsoft.AspNetCore.Components.Authorization;
using ProductShopping.UI.Blazor.Contracts;
using System.Security.Claims;
using System.Text.Json;

namespace ProductShopping.UI.Blazor.Services.Auth;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore tokenStore;
    private ClaimsPrincipal currentUser = new(new ClaimsIdentity());

    public JwtAuthenticationStateProvider(ITokenStore tokenStore)
    {
        this.tokenStore = tokenStore;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(currentUser));
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        token = NormalizeToken(token);

        await tokenStore.SetAccessTokenAsync(token);

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));
    }

    public Task RestoreUserFromTokenAsync(string token)
    {
        token = NormalizeToken(token);

        if (string.IsNullOrWhiteSpace(token))
        {
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }
        else
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            currentUser = new ClaimsPrincipal(identity);
        }

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));

        return Task.CompletedTask;
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await tokenStore.RemoveAccessTokenAsync();

        currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));
    }

    private static string NormalizeToken(string token)
    {
        token = token.Trim();

        if (token.StartsWith("\"") && token.EndsWith("\""))
        {
            token = JsonSerializer.Deserialize<string>(token) ?? string.Empty;
        }

        return token.Trim();
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        var parts = jwt.Split('.');
        if (parts.Length != 3)
        {
            return claims;
        }

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)
                            ?? new Dictionary<string, JsonElement>();

        foreach (var kvp in keyValuePairs)
        {
            if (kvp.Key == ClaimTypes.Role || kvp.Key == "role")
            {
                if (kvp.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in kvp.Value.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString()));
                }

                continue;
            }

            claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}