using System.Net.Http.Headers;
using ProductShopping.Api.EndToEndTests.Authentication;

namespace ProductShopping.Api.EndToEndTests.Extensions;

public static class HttpClientAuthExtensions
{
    public static HttpClient AuthenticateAs(
        this HttpClient client,
        string userId = "1",
        string role = "User",
        string email = "test@user.com")
    {
        var token = TestJwtTokenProvider.GenerateToken(userId, email, role);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    public static HttpClient AuthenticateAsUser(this HttpClient client, string userId = "1")
    {
        return client.AuthenticateAs(userId, "User");
    }

    public static HttpClient AuthenticateAsAdministrator(this HttpClient client, string userId = "1")
    {
        return client.AuthenticateAs(userId, "Administrator");
    }

    public static HttpClient WithoutAuthentication(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
        return client;
    }
}