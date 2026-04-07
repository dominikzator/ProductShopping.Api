using ProductShopping.Api.EndToEndTests.Extensions;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Features.Order.Commands.DeleteOrder;
using ProductShopping.Application.Features.Order.Commands.UpdateOrder;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Domain.Models;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ProductShopping.Api.EndToEndTests.Tests;

[Collection("IntegrationTests")]
public class OrdersControllerTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task ResetStateAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.WithoutAuthentication();
    }

    [Fact]
    public async Task GetOrders_ShouldReturnAllOrders_WhenUserIsAuthenticated()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/orders");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<OrderDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);

        result.Metadata.ShouldNotBeNull();
        result.Metadata.CurrentPage.ShouldBe(1);
        result.Metadata.PageSize.ShouldBe(10);
        result.Metadata.TotalCount.ShouldBe(3);
        result.Metadata.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnSecondPage_WhenPaginationIsProvided()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/orders?pageNumber=2&pageSize=2");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<OrderDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(1);

        result.Metadata.ShouldNotBeNull();
        result.Metadata.CurrentPage.ShouldBe(2);
        result.Metadata.PageSize.ShouldBe(2);
        result.Metadata.TotalCount.ShouldBe(3);
        result.Metadata.TotalPages.ShouldBe(2);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOrder_WhenOrderExists()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/orders/1");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var order = await response.Content.ReadFromJsonAsync<OrderDto>();

        order.ShouldNotBeNull();
        order.Id.ShouldBe(1);
        order.OrderNumber.ShouldBe("1");
        order.OwnerId.ShouldBe("1");
    }

    [Fact]
    public async Task GetOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/orders/999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldCreateOrder_WhenUserIsAuthenticated()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new CreateOrderCommand
        {
            Address = new Address
            {
                Street = "Test Street",
                BuildingNumber = "10",
                ApartmentNumber = "2",
                City = "Wroclaw",
                PostalCode = "50-001",
                Country = "Poland",
                PhoneNumber = "123-456-789"
            }
        };

        var response = await _client.PostAsJsonAsync("/api/orders", command);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();

        createdOrder.ShouldNotBeNull();
        createdOrder.Id.ShouldBeGreaterThan(0);
        createdOrder.OwnerId.ShouldBe("1");
        createdOrder.Street.ShouldBe("Test Street");
        createdOrder.City.ShouldBe("Wroclaw");
        createdOrder.Country.ShouldBe("Poland");
        createdOrder.PaymentUrl.ShouldNotBeEmpty();

        var getAllResponse = await _client.GetAsync("/api/orders");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<OrderDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(4);
    }

    [Fact]
    public async Task Put_ShouldUpdateOrder_WhenUserIsAdministrator()
    {
        await ResetStateAsync();
        _client.AuthenticateAsAdministrator();

        var command = new UpdateOrderCommand
        {
            Id = 1,
            Address = new Address
            {
                Street = "Updated Street",
                BuildingNumber = "20",
                ApartmentNumber = "5",
                City = "Updated City",
                PostalCode = "98-765",
                Country = "Updated Country",
                PhoneNumber = "987-654-321"
            }
        };

        var response = await _client.PutAsJsonAsync("/api/orders/1", command);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        _client.AuthenticateAsUser();

        var getResponse = await _client.GetAsync("/api/orders/1");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updatedOrder = await getResponse.Content.ReadFromJsonAsync<OrderDto>();

        updatedOrder.ShouldNotBeNull();
        updatedOrder.Id.ShouldBe(1);
        updatedOrder.Street.ShouldBe("Updated Street");
        updatedOrder.City.ShouldBe("Updated City");
        updatedOrder.Country.ShouldBe("Updated Country");
    }

    [Fact]
    public async Task Delete_ShouldRemoveOrder_WhenUserIsAdministrator()
    {
        await ResetStateAsync();

        var adminClient = _factory.CreateClient();
        adminClient.AuthenticateAsAdministrator();

        var deleteResponse = await adminClient.DeleteAsync("/api/orders/1");

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var userClient = _factory.CreateClient();
        userClient.AuthenticateAsUser();

        var getDeletedResponse = await userClient.GetAsync("/api/orders/1");
        getDeletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var getAllResponse = await userClient.GetAsync("/api/orders");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<OrderDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/orders");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnForbidden_WhenUserDoesNotHaveUserRole()
    {
        await ResetStateAsync();
        _client.AuthenticateAsAdministrator();

        var response = await _client.GetAsync("/api/orders");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Put_ShouldReturnForbidden_WhenUserIsNotAdministrator()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new UpdateOrderCommand
        {
            Id = 1,
            Address = new Address()
        };

        var response = await _client.PutAsJsonAsync("/api/orders/1", command);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenUserIsNotAdministrator()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new DeleteOrderCommand
        {
            Id = 1
        };

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/orders/1")
        {
            Content = JsonContent.Create(command)
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}