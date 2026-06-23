using Microsoft.JSInterop;
using ProductShopping.UI.Blazor.Contracts;

namespace ProductShopping.UI.Blazor.Handlers
{
    public sealed class BrowserTokenStore : ITokenStore
    {
        private const string AccessTokenKey = "authToken";
        private readonly IJSRuntime jsRuntime;

        public BrowserTokenStore(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async ValueTask<string?> GetAccessTokenAsync()
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);
        }

        public async ValueTask SetAccessTokenAsync(string token)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, token);
        }

        public async ValueTask RemoveAccessTokenAsync()
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        }
    }
}
