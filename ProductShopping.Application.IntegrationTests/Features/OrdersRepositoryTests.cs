using ProductShopping.Application.IntegrationTests.Mocks;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.Repositories;
using Shouldly;

namespace ProductShopping.Application.IntegrationTests.Features;

public class OrdersRepositoryTests
{
    [Fact]
    public async Task OrdersRepository_GetOrders()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var result = await ordersRepo.GetAsync();

        result.Count().ShouldBe(3);
    }
    [Fact]
    public async Task OrdersRepository_GetOrderWithInvalidId()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var result = await ordersRepo.GetByIdAsync(-1);

        result.ShouldBeNull();
    }
    [Fact]
    public async Task OrdersRepository_GetOrderWithValidId()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var result = await ordersRepo.GetByIdAsync(1);

        result.ShouldNotBeNull();
    }
    [Fact]
    public async Task OrdersRepository_GetOrderItemWithInvalidId()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var result = await ordersRepo.GetUserOrderItemAsync("1", -1);

        result.ShouldBeNull();
    }
    [Fact]
    public async Task OrdersRepository_GetOrderItemWithValidId()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);
        var result = await ordersRepo.GetUserOrderItemAsync("1", 1);

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task OrdersRepository_CreateOrder()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var testingOrderNumber = "OrderNumber-1";

        var orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);

        var order = await ordersRepo.GetUserOrderByOrderNumberAsync("1", testingOrderNumber);
        order.ShouldBeNull();

        await ordersRepo.CreateAsync(new Order
        {
            CustomerId = "1",
            Address = new Address
            {

            },
            OrderNumber = testingOrderNumber
        });

        orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(4);

        order = await ordersRepo.GetUserOrderByOrderNumberAsync("1", testingOrderNumber);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(4);
    }
    [Fact]
    public async Task OrdersRepository_UpdateOrder()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var order = await ordersRepo.GetUserOrderAsync("1", 1);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(1);

        var orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);

        var newCity = "New York";

        order.Address.City.ShouldNotBe(newCity);
        order.Address.City = newCity;
        await ordersRepo.UpdateAsync(order);

        orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);

        order = await ordersRepo.GetUserOrderAsync("1", 1);
        order.ShouldNotBeNull();
        order.Address.City.ShouldBe(newCity);
        order.Id.ShouldBe(1);
    }
    [Fact]
    public async Task OrdersRepository_RemoveOrder()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, setup.Item2.Object);

        var order = await ordersRepo.GetUserOrderAsync("1", 1);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(1);

        var orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);

        await ordersRepo.DeleteAsync(order);

        orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(2);

        var getOrderAgain = await ordersRepo.GetUserOrderAsync("1", 1);
        getOrderAgain.ShouldBeNull();
    }
}
