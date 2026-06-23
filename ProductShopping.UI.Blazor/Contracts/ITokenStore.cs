namespace ProductShopping.UI.Blazor.Contracts;

public interface ITokenStore
{
    ValueTask<string?> GetAccessTokenAsync();
    ValueTask SetAccessTokenAsync(string token);
    ValueTask RemoveAccessTokenAsync();
}