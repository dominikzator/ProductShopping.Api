using ProductShopping.Api.EndToEndTests.Extensions;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItems;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Models.Paging;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ProductShopping.Api.EndToEndTests.Tests;

[Collection("IntegrationTests")]
public class CartControllerTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CartControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetCartItems_ShouldReturnAllCartItems_WhenUserIsAuthenticated()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/cart");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

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
    public async Task GetCartItems_ShouldReturnSecondPage_WhenPaginationIsProvided()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/cart?pageNumber=2&pageSize=2");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

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
    public async Task GetCartItem_ShouldReturnCartItem_WhenCartItemExists()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/cart/1");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var cartItem = await response.Content.ReadFromJsonAsync<CartItemDto>();

        cartItem.ShouldNotBeNull();
        cartItem.Id.ShouldBe(1);
    }
    [Fact]
    public async Task GetCartItem_ShouldReturnNotFound_WhenCartItemDoesNotExist()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var response = await _client.GetAsync("/api/cart/999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task Post_ShouldAddCartItem_WhenUserIsAuthenticated()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new AddCartItemCommand
        {
            ProductId = 4,
            Quantity = 2
        };

        var response = await _client.PostAsJsonAsync("/api/cart", command);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var addedCartItem = await response.Content.ReadFromJsonAsync<CartItemDto>();

        addedCartItem.ShouldNotBeNull();
        addedCartItem.ProductId.ShouldBe(4);
        addedCartItem.Quantity.ShouldBe(2);

        var getAllResponse = await _client.GetAsync("/api/cart");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(4);
        result.Data.Any(x => x.ProductId == 4 && x.Quantity == 2).ShouldBeTrue();
    }
    [Fact]
    public async Task Post_ShouldIncreaseQuantity_WhenAddingSameProductTwice_AndTotalQuantityShouldMatch()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var firstCommand = new AddCartItemCommand
        {
            ProductId = 4,
            Quantity = 2
        };

        var secondCommand = new AddCartItemCommand
        {
            ProductId = 4,
            Quantity = 3
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/cart", firstCommand);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var secondResponse = await _client.PostAsJsonAsync("/api/cart", secondCommand);
        secondResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var getAllResponse = await _client.GetAsync("/api/cart");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        var item = result.Data.Single(x => x.ProductId == 4);
        item.Quantity.ShouldBe(5);

        var totalQuantity = result.Data.Sum(x => x.Quantity);
        totalQuantity.ShouldBe(result.Data.Where(x => x.ProductId != 4).Sum(x => x.Quantity) + 5);
    }
    [Fact]
    public async Task Remove_ShouldDecreaseCartItemQuantity_WhenRemovingLessThanExistingQuantity()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new RemoveCartItemCommand
        {
            CartItemId = 2,
            Quantity = 1
        };

        var response = await _client.DeleteAsJsonAsync("/api/cart", command);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/api/cart/2");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var cartItem = await getResponse.Content.ReadFromJsonAsync<CartItemDto>();

        cartItem.ShouldNotBeNull();
        cartItem.Id.ShouldBe(2);
        cartItem.Quantity.ShouldBe(1);

        var getAllResponse = await _client.GetAsync("/api/cart");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
    }
    [Fact]
    public async Task Remove_ShouldDeleteCartItem_WhenRemovingExactExistingQuantity()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new RemoveCartItemCommand
        {
            CartItemId = 2,
            Quantity = 2
        };

        var response = await _client.DeleteAsJsonAsync("/api/cart", command);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/api/cart/2");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var getAllResponse = await _client.GetAsync("/api/cart");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
        result.Data.Any(x => x.Id == 2).ShouldBeFalse();
    }
    [Fact]
    public async Task Remove_ShouldReturnBadRequest_WhenRemovingMoreThanExistingQuantity()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new RemoveCartItemCommand
        {
            CartItemId = 2,
            Quantity = 3
        };

        var response = await _client.DeleteAsJsonAsync("/api/cart", command);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var getResponse = await _client.GetAsync("/api/cart/2");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var cartItem = await getResponse.Content.ReadFromJsonAsync<CartItemDto>();

        cartItem.ShouldNotBeNull();
        cartItem.Id.ShouldBe(2);
        cartItem.Quantity.ShouldBe(2);

        var getAllResponse = await _client.GetAsync("/api/cart");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
    }
    [Fact]
    public async Task RemoveAll_ShouldClearCart_WhenUserIsAuthenticated()
    {
        await ResetStateAsync();
        _client.AuthenticateAsUser();

        var command = new RemoveCartItemsCommand();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/cart/clear")
        {
            Content = JsonContent.Create(command)
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getAllResponse = await _client.GetAsync("/api/cart");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<CartItemDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(0);
    }
    [Fact]
    public async Task Remove_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        await ResetStateAsync();

        var response = await _client.DeleteAsync("/api/cart?cartItemId=2&quantity=1");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Remove_ShouldReturnForbidden_WhenUserDoesNotHaveUserRole()
    {
        await ResetStateAsync();
        _client.AuthenticateAsAdministrator();

        var response = await _client.DeleteAsync("/api/cart?cartItemId=2&quantity=1");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}