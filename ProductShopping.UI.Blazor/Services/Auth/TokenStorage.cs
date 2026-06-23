using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ProductShopping.UI.Blazor.Contracts;

namespace ProductShopping.UI.Blazor.Authentication;

public sealed class ProtectedSessionTokenStorage : ITokenStore
{
    private const string AccessTokenKey = "authToken";
    private readonly ProtectedSessionStorage protectedSessionStorage;

    public ProtectedSessionTokenStorage(ProtectedSessionStorage protectedSessionStorage)
    {
        this.protectedSessionStorage = protectedSessionStorage;
    }

    public async ValueTask<string?> GetAccessTokenAsync()
    {
        try
        {
            var result = await protectedSessionStorage.GetAsync<string>(AccessTokenKey);

            if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
            {
                return null;
            }

            return result.Value;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async ValueTask SetAccessTokenAsync(string token)
    {
        await protectedSessionStorage.SetAsync(AccessTokenKey, token);
    }

    public async ValueTask RemoveAccessTokenAsync()
    {
        await protectedSessionStorage.DeleteAsync(AccessTokenKey);
    }
}