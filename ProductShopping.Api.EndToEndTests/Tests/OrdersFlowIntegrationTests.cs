/*namespace ProductShopping.Api.EndToEndTests.Tests;

using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using Shouldly;
using System.Net.Http.Json;
using Xunit;

public class OrdersFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersFlowIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task User_Can_Add_Product_To_Cart_And_Create_Order()
    {
        var addCartItemRequest = new
        {
            productId = 7,
            quantity = 2
        };

        var addCartItemResponse = await _client.PostAsJsonAsync("/api/cartItems", addCartItemRequest);
        addCartItemResponse.EnsureSuccessStatusCode();

        var createOrderRequest = new
        {
            street = "Main Street",
            buildingNumber = "1",
            apartmentNumber = "2",
            city = "Wroclaw",
            postalCode = "50-001",
            country = "Poland",
            phoneNumber = "123456789"
        };

        var createOrderResponse = await _client.PostAsJsonAsync("/api/orders", createOrderRequest);
        createOrderResponse.EnsureSuccessStatusCode();

        var createdOrder = await createOrderResponse.Content.ReadFromJsonAsync<OrderDto>();

        createdOrder.ShouldNotBeNull();
        createdOrder.Id.ShouldBeGreaterThan(0);
        createdOrder.OrderItems.Count.ShouldBeGreaterThan(0);

        var getOrderResponse = await _client.GetAsync($"/api/orders/{createdOrder.Id}");
        getOrderResponse.EnsureSuccessStatusCode();

        var fetchedOrder = await getOrderResponse.Content.ReadFromJsonAsync<OrderDto>();

        fetchedOrder.ShouldNotBeNull();
        fetchedOrder.Id.ShouldBe(createdOrder.Id);
        fetchedOrder.OrderItems.Count.ShouldBe(1);
    }
}*/