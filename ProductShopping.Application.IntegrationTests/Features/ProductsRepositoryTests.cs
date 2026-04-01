using AutoMapper;
using Moq;
using ProductShopping.Application.IntegrationTests.Mocks;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.Repositories;
using Shouldly;

namespace ProductShopping.Application.IntegrationTests.Features;

public class ProductsRepositoryTests
{
    [Fact]
    public async Task ProductsRepository_GetProducts()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, setup.Item2.Object);

        var result = await productsRepo.GetAsync();

        result.Count().ShouldBe(7);
    }
    [Fact]
    public async Task ProductsRepository_GetProductWithInvalidId()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, setup.Item2.Object);

        var result = await productsRepo.GetByIdAsync(-1);

        result.ShouldBeNull();
    }
    [Fact]
    public async Task ProductsRepository_GetProductWithValidId()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, setup.Item2.Object);

        var result = await productsRepo.GetByIdAsync(1);

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task ProductsRepository_CreateProduct()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, setup.Item2.Object);

        productsRepo.GetAsync().Result.Count().ShouldBe(7);

        await productsRepo.CreateAsync(new Product
        {
            Name = "Added Product",
            CategoryId = 1,
            Price = 1,
            Rating = 1
        });

        var timeNow = DateTime.UtcNow.ToShortDateString();

        var product = await productsRepo.GetProductByNameAsync("Added Product");

        product.ShouldNotBeNull();
        productsRepo.GetAsync().Result.Count().ShouldBe(8);
        product.Id.ShouldBe(8);
        product.CategoryId.ShouldBe(1);
        product.Price.ShouldBe(1);
        product.Rating.ShouldBe(1);
        product.Name.ShouldBe("Added Product");
        product.DateCreated.ShouldNotBeNull();
        product.DateModified.ShouldNotBeNull();
    }

    [Fact]
    public async Task ProductsRepository_UpdateProduct()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, setup.Item2.Object);

        var product = await productsRepo.GetByIdAsync(1);
        var dateModified = product.DateModified;

        product.Name.ShouldBe("Little Canoli");

        product.Name = "Gigantic Canoli";
        await productsRepo.UpdateAsync(product);


        product.ShouldNotBeNull();
        productsRepo.GetAsync().Result.Count().ShouldBe(7);
        product.CategoryId.ShouldBe(1);
        product.Price.ShouldBe((decimal)2.49);
        product.Name.ShouldBe("Gigantic Canoli");
        product.DateCreated.ShouldNotBeNull();
        product.DateModified.ShouldNotBeNull();
        product.DateModified.ShouldNotBe(dateModified);
    }
    [Fact]
    public async Task ProductsRepository_DeleteProduct()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, setup.Item2.Object);

        var product = await productsRepo.GetByIdAsync(1);
        await productsRepo.DeleteAsync(product);

        var getProductAgain = await productsRepo.GetByIdAsync(1);

        productsRepo.GetAsync().Result.Count().ShouldBe(6);
        getProductAgain.ShouldBeNull();
    }
}
