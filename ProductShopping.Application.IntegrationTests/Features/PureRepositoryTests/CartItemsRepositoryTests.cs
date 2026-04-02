using Castle.Core.Logging;
using Moq;
using ProductShopping.Application.IntegrationTests.Mocks;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.Repositories;
using Shouldly;

namespace ProductShopping.Application.IntegrationTests.Features.PureRepositoryTests;

public class CartItemsRepositoryTests
{
    [Fact]
    public async Task CartItemsRepository_GetCartItems()
    {
        var setup = await CartItemsDbMocks.CreateInMemoryContextSetup();
        var loggerMock = new Mock<ILogger>();
        var cartsRepo = new CartsRepository(setup.Item1, setup.Item2.Object);

        var result = await cartsRepo.GetUserCartItemsAsync("1");

        result.Count().ShouldBe(3);
    }
    [Fact]
    public async Task CartItemsRepository_GetCartItemWithInvalidId()
    {
        var setup = await CartItemsDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, setup.Item2.Object);

        var result = await cartsRepo.GetUserCartItemAsync("1", -1);

        result.ShouldBeNull();
    }
    [Fact]
    public async Task CartItemsRepository_GetCartItemWithValidId()
    {
        var setup = await CartItemsDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, setup.Item2.Object);
        var result = await cartsRepo.GetUserCartItemAsync("1", 1);

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task CartItemsRepository_AddCartItem()
    {
        var setup = await CartItemsDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, setup.Item2.Object);

        cartsRepo.GetUserCartItemsAsync("1").Result.Count.ShouldBe(3);

        await cartsRepo.CreateCartItemAsync(new CartItem
        {
            CartId = 1,
            ProductId = 4,
            Quantity = 4
        });

        var timeNow = DateTime.UtcNow.ToShortDateString();

        var cartItem = await cartsRepo.GetUserCartItemAsync("1", 4);

        cartItem.ShouldNotBeNull();
        cartsRepo.GetUserCartItemsAsync("1").Result.Count().ShouldBe(4);
        cartItem.Id.ShouldBe(4);
        cartItem.CartId.ShouldBe(1);
        cartItem.ProductId.ShouldBe(4);
        cartItem.Quantity.ShouldBe(4);
        cartItem.DateCreated.ShouldNotBeNull();
        cartItem.DateModified.ShouldNotBeNull();
    }
    [Fact]
    public async Task CartItemsRepository_RemoveCartItem()
    {
        var setup = await CartItemsDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, setup.Item2.Object);

        var cartItems = await cartsRepo.GetUserCartItemsAsync("1");
        cartItems.Count.ShouldBe(3);

        var cartItem = await cartsRepo.GetUserCartItemAsync("1", 1);
        await cartsRepo.DeleteCartItemAsync("1", cartItem.Id);

        var getCartItemsAgain = await cartsRepo.GetUserCartItemsAsync("1");
        getCartItemsAgain.Count.ShouldBe(2);

        var getCartItemAgain = await cartsRepo.GetUserCartItemAsync("1", 1);
        getCartItemAgain.ShouldBeNull();
    }
}
