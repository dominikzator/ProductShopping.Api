using System.Net.Http.Json;

namespace ProductShopping.Api.EndToEndTests.Extensions;

public static class HttpClientJsonExtensions
{
    public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T value)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = JsonContent.Create(value)
        };

        return client.SendAsync(request);
    }
}