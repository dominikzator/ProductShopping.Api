using Microsoft.AspNetCore.Components.Authorization;
using ProductShopping.UI.Blazor.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore TokenStore;

    private static readonly ClaimsPrincipal Anonymous =
        new(new ClaimsIdentity());

    private ClaimsPrincipal currentUser = Anonymous;

    public JwtAuthenticationStateProvider(ITokenStore tokenStore)
    {
        TokenStore = tokenStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var raw = await TokenStore.GetAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(raw))
        {
            currentUser = Anonymous;
            return new AuthenticationState(currentUser);
        }

        AccessTokenEnvelope? tokenEnvelope;

        try
        {
            tokenEnvelope = JsonSerializer.Deserialize<AccessTokenEnvelope>(raw);
        }
        catch
        {
            currentUser = Anonymous;
            return new AuthenticationState(currentUser);
        }

        if (tokenEnvelope is null || !tokenEnvelope.isSuccess || string.IsNullOrWhiteSpace(tokenEnvelope.value))
        {
            currentUser = Anonymous;
            return new AuthenticationState(currentUser);
        }

        currentUser = BuildUserFromToken(tokenEnvelope.value);

        return new AuthenticationState(currentUser);
    }

    public async Task MarkUserAsAuthenticatedAsync(string tokenResponseJson)
    {
        await TokenStore.SetAccessTokenAsync(tokenResponseJson);

        var tokenEnvelope = JsonSerializer.Deserialize<AccessTokenEnvelope>(tokenResponseJson);

        if (tokenEnvelope is null || string.IsNullOrWhiteSpace(tokenEnvelope.value))
        {
            currentUser = Anonymous;
        }
        else
        {
            currentUser = BuildUserFromToken(tokenEnvelope.value);
        }

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await TokenStore.RemoveAccessTokenAsync();
        currentUser = Anonymous;

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));
    }

    private static ClaimsPrincipal BuildUserFromToken(string jwt)
    {
        var claims = ParseClaimsFromJwt(jwt);
        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return new ClaimsPrincipal(identity);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}

public sealed class AccessTokenEnvelope
{
    public bool isSuccess { get; set; }
    public string? value { get; set; }
    public List<string>? errors { get; set; }
}