using Microsoft.AspNetCore.Http;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductShopping.Application.IntegrationTests.Mocks;

public class HttpContextAccessorMock
{
    public static IHttpContextAccessor BuildHttpContextAccessorWithUserId(string userId = "1")
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        return accessorMock.Object;
    }
}
