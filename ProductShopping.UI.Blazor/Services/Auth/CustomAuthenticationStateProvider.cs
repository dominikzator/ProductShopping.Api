using Microsoft.AspNetCore.Components.Authorization;
using ProductShopping.UI.Blazor.Contracts;
using ProductShopping.UI.Shared.Contracts;
using System.Security.Claims;

namespace ProductShopping.UI.Blazor.Services.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous =
        new(new ClaimsIdentity());

    private readonly ITokenStorage _tokenStorage;
    private readonly IJwtClaimsFactory _jwtClaimsFactory;

    public CustomAuthenticationStateProvider(
        ITokenStorage tokenStorage,
        IJwtClaimsFactory jwtClaimsFactory)
    {
        _tokenStorage = tokenStorage;
        _jwtClaimsFactory = jwtClaimsFactory;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _tokenStorage.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = _jwtClaimsFactory.CreateClaimsFromRawToken(token);
            var identity = new ClaimsIdentity(claims, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (InvalidOperationException)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task MarkUserAsAuthenticatedAsync(string rawToken, string? fallbackEmail = null)
    {
        var token = _jwtClaimsFactory.ExtractAccessToken(rawToken);
        await _tokenStorage.SetTokenAsync(token);

        var claims = _jwtClaimsFactory.CreateClaimsFromRawToken(token, fallbackEmail);
        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _tokenStorage.RemoveTokenAsync();

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(Anonymous)));
    }
}