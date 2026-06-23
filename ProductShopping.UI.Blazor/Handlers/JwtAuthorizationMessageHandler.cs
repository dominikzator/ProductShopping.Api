using ProductShopping.UI.Blazor.Contracts;
using System.Net.Http.Headers;

namespace ProductShopping.UI.Blazor.Handlers;

public sealed class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenStore tokenStore;

    public JwtAuthorizationMessageHandler(ITokenStore tokenStore)
    {
        this.tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}