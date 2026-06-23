using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ProductShopping.UI.Blazor.Contracts;

namespace ProductShopping.UI.Blazor.Services.Auth;

public class ProtectedSessionTokenStorage : ITokenStorage
{
    private const string TokenKey = "auth_token";
    private readonly ProtectedSessionStorage _storage;

    public ProtectedSessionTokenStorage(ProtectedSessionStorage storage)
    {
        _storage = storage;
    }

    public async Task<string?> GetTokenAsync()
    {
        var result = await _storage.GetAsync<string>(TokenKey);
        return result.Success ? result.Value : null;
    }

    public async Task SetTokenAsync(string token)
    {
        await _storage.SetAsync(TokenKey, token);
    }

    public async Task RemoveTokenAsync()
    {
        await _storage.DeleteAsync(TokenKey);
    }
}