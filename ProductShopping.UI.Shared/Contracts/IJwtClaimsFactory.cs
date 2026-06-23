using System.Security.Claims;

namespace ProductShopping.UI.Shared.Contracts;

public interface IJwtClaimsFactory
{
    List<Claim> CreateClaimsFromRawToken(string rawToken, string? fallbackEmail = null);
    string ExtractAccessToken(string rawToken);
}
