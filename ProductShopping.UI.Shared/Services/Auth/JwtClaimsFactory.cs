using ProductShopping.UI.Shared.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductShopping.UI.Shared.Services.Auth;

public class JwtClaimsFactory : IJwtClaimsFactory
{
    public List<Claim> CreateClaimsFromRawToken(string rawToken, string? fallbackEmail = null)
    {
        var jwtToken = ExtractAccessToken(rawToken);

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(jwtToken))
        {
            throw new InvalidOperationException("API did not return a valid JWT token.");
        }

        JwtSecurityToken jwt;

        try
        {
            jwt = handler.ReadJwtToken(jwtToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Invalid JWT token format.", ex);
        }

        var claims = new List<Claim>();

        foreach (var claim in jwt.Claims)
        {
            var mappedType = claim.Type switch
            {
                "email" => ClaimTypes.Email,
                "unique_name" => ClaimTypes.Name,
                "role" => ClaimTypes.Role,
                _ => claim.Type
            };

            claims.Add(new Claim(mappedType, claim.Value));
        }

        claims.Add(new Claim("access_token", jwtToken));

        if (!claims.Any(c => c.Type == ClaimTypes.Name))
        {
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? fallbackEmail;

            if (!string.IsNullOrWhiteSpace(email))
            {
                claims.Add(new Claim(ClaimTypes.Name, email));
            }
        }

        return claims;
    }

    public string ExtractAccessToken(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            throw new InvalidOperationException("Token is empty.");
        }

        var jwtToken = rawToken.Trim();

        if (jwtToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            jwtToken = jwtToken["Bearer ".Length..].Trim();
        }

        return jwtToken;
    }
}