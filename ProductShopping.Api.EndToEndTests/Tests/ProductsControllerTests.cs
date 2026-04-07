using ProductShopping.Api.EndToEndTests.Extensions;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Commands.UpdateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Models.Paging;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ProductShopping.Api.EndToEndTests.Tests;

[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_ShouldReturnAllProducts_OnFirstPage()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(7);
        result.Data.Any(x => x.Name == "Milk").ShouldBeTrue();
        result.Data.Any(x => x.Name == "Car Cleaner").ShouldBeTrue();

        result.Metadata.ShouldNotBeNull();
        result.Metadata.CurrentPage.ShouldBe(1);
        result.Metadata.PageSize.ShouldBe(10);
        result.Metadata.TotalCount.ShouldBe(7);
        result.Metadata.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSecondPage_WhenPaginationIsProvided()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products?pageNumber=2&pageSize=3");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);

        result.Metadata.ShouldNotBeNull();
        result.Metadata.CurrentPage.ShouldBe(2);
        result.Metadata.PageSize.ShouldBe(3);
        result.Metadata.TotalCount.ShouldBe(7);
        result.Metadata.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnLastPageWithOneItem_WhenPaginationTargetsLastPage()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products?pageNumber=3&pageSize=3");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(1);

        result.Metadata.CurrentPage.ShouldBe(3);
        result.Metadata.PageSize.ShouldBe(3);
        result.Metadata.TotalCount.ShouldBe(7);
        result.Metadata.TotalPages.ShouldBe(3);
    }

    [Fact]
    public async Task GetProducts_ShouldFilterByName()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products?search=Milk");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(1);
        result.Data.First().Name.ShouldBe("Milk");
    }

    [Fact]
    public async Task GetProducts_ShouldFilterByCategory()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products?categoryName=Food");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(4);
        result.Data.All(x => x.CategoryName == "Food").ShouldBeTrue();
    }

    [Fact]
    public async Task GetProducts_ShouldFilterByPriceRange()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products?minPrice=1.00&maxPrice=3.00");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
        result.Data.All(x => x.Price >= 1.00m && x.Price <= 3.00m).ShouldBeTrue();
    }

    [Fact]
    public async Task GetProducts_ShouldFilterByMinimumRating()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products?minRating=4");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
        result.Data.All(x => x.Rating >= 4).ShouldBeTrue();
    }

    [Fact]
    public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
    {
        await ResetStateAsync();

        var response = await _client.GetAsync("/api/products/1");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();

        product.ShouldNotBeNull();
        product.Id.ShouldBe(1);
        product.Name.ShouldBe("Little Canoli");
        product.CategoryName.ShouldBe("Food");
    }

    [Fact]
    public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var response = await _client.GetAsync("/api/products/999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldCreateProduct_WhenUserIsAdministrator()
    {
        await ResetStateAsync();

        _client.AuthenticateAsAdministrator();

        var command = new CreateProductCommand
        {
            Name = "Test Product",
            CategoryName = "Food",
            Price = 15.99m,
            Rating = 4.6
        };
        var response = await _client.PostAsJsonAsync("/api/products", command);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();

        var getCreatedResponse = await _client.GetAsync(response.Headers.Location);
        getCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var createdProduct = await getCreatedResponse.Content.ReadFromJsonAsync<ProductDto>();

        createdProduct.ShouldNotBeNull();
        createdProduct.Name.ShouldBe("Test Product");
        createdProduct.CategoryName.ShouldBe("Food");
        createdProduct.Price.ShouldBe(15.99m);
        createdProduct.Rating.ShouldBe(4.6);
    }

    [Fact]
    public async Task Put_ShouldUpdateProduct_WhenUserIsAdministrator()
    {
        await ResetStateAsync();

        _client.AuthenticateAsAdministrator();

        var command = new UpdateProductCommand
        {
            Id = 1,
            Name = "Little Canoli Updated",
            CategoryName = "Automotive",
            Price = 9.99m,
            Rating = 4.9
        };

        var response = await _client.PutAsJsonAsync("/api/products/1", command);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/api/products/1");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();

        updatedProduct.ShouldNotBeNull();
        updatedProduct.Id.ShouldBe(1);
        updatedProduct.Name.ShouldBe("Little Canoli Updated");
        updatedProduct.CategoryName.ShouldBe("Automotive");
        updatedProduct.Price.ShouldBe(9.99m);
        updatedProduct.Rating.ShouldBe(4.9);
    }

    [Fact]
    public async Task Delete_ShouldRemoveProduct_WhenUserIsAdministrator()
    {
        await ResetStateAsync();

        _client.AuthenticateAsAdministrator();

        var response = await _client.DeleteAsync("/api/products/1");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getDeletedResponse = await _client.GetAsync("/api/products/1");
        getDeletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var getAllResponse = await _client.GetAsync("/api/products");
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await getAllResponse.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();

        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(6);
    }

    private async Task ResetStateAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client.WithoutAuthentication();
    }
}
