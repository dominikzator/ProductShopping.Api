using Microsoft.Extensions.Configuration;
using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Features.Order.Commands.DeleteOrder;
using ProductShopping.Application.Features.Order.Commands.UpdateOrder;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Features.Order.Queries.GetOrders;
using ProductShopping.Application.IntegrationTests.Helpers;
using ProductShopping.Application.IntegrationTests.Mocks;
using ProductShopping.Persistence.Repositories;
using Shouldly;

namespace ProductShopping.Application.IntegrationTests.Features.HandlersRepositoryTests;

public class OrdersHandlersIntegrationTests
{
    [Fact]
    public async Task OrdersHandlersIntegrationTests_GetOrders()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {

            }
        };

        var handler = new GetOrderListQueryHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count().ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalPages.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_GetOrdersWithCustomPagination()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 2,
                PageSize = 2
            }
        };

        var handler = new GetOrderListQueryHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(2);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(2);
        result.Value.Metadata.TotalPages.ShouldBe(2);
        result.Value.Metadata.TotalCount.ShouldBe(3);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_GetOrdersWithCustomPagination1PerPageFirstPage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 1,
                PageSize = 1
            }
        };

        var handler = new GetOrderListQueryHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.TotalCount.ShouldBe(3);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_GetOrdersWithCustomPagination1PerPageMiddlePage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 2,
                PageSize = 1
            }
        };

        var handler = new GetOrderListQueryHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(2);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.TotalCount.ShouldBe(3);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_GetOrdersWithCustomPagination1PerPageLastPage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 3,
                PageSize = 1
            }
        };

        var handler = new GetOrderListQueryHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(3);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.TotalCount.ShouldBe(3);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_GetOrderWithValidId()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var orderQuery = new GetOrderDetailQuery
        {
            Id = 1
        };

        var handler = new GetOrderDetailQueryHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(orderQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(1);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_CreateOrder()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        var paymentsServiceMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var orderCommand = new CreateOrderCommand
        {
            Address = new Domain.Models.Address
            {

            }
        };

        var orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);


        var handler = new CreateOrderCommandHandler(ordersRepo, cartsRepo, usersServiceMock.Object, paymentsServiceMock.Object, configMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(orderCommand, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();

        orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(4);

        var order = await ordersRepo.GetByIdAsync(4);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(4);
        order.OrderNumber.ShouldNotBeEmpty();
        order.OrderStatus.ShouldBe(Domain.Enums.OrderStatus.Pending);
        order.CustomerId.ShouldBe("1");
        order.CreatedBy.ShouldNotBeEmpty();
        order.DateCreated.ShouldNotBeNull();
        order.DateModified.ShouldNotBeNull();
        order.Address.ShouldNotBeNull();
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_UpdateOrder()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        var paymentsServiceMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var cityAddressUpdate = "New York";

        var orderCommand = new UpdateOrderCommand
        {
            Id = 1,
            Address = new Domain.Models.Address
            {
                City = cityAddressUpdate
            },
        };

        var orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);

        var order = await ordersRepo.GetByIdAsync(1);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(1);
        order.Address.ShouldNotBeNull();
        order.Address.City.ShouldNotBe(cityAddressUpdate);


        var handler = new UpdateOrderCommandHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(orderCommand, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();

        orders = await ordersRepo.GetUserOrdersAsync("1");
        orders.Count.ShouldBe(3);

        order = await ordersRepo.GetByIdAsync(1);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(1);
        order.Address.ShouldNotBeNull();
        order.Address.City.ShouldBe(cityAddressUpdate);
    }
    [Fact]
    public async Task OrdersHandlersIntegrationTests_RemoveOrder()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var ordersRepo = new OrdersRepository(setup.Item1, AutoMapperHelper.Create());
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        var paymentsServiceMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var order = await ordersRepo.GetByIdAsync(1);
        order.ShouldNotBeNull();
        order.Id.ShouldBe(1);

        var orderItems = await ordersRepo.GetUserOrderItemsByOrderIdAsync("1", 1);
        orderItems.ShouldNotBeNull();
        orderItems.Count().ShouldBe(1);

        var orderCommand = new DeleteOrderCommand
        {
            Id = 1
        };

        var handler = new DeleteOrderCommandHandler(ordersRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(orderCommand, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();

        order = await ordersRepo.GetByIdAsync(1);
        order.ShouldBeNull();

        orderItems = await ordersRepo.GetUserOrderItemsByOrderIdAsync("1", 1);
        orderItems.Count.ShouldBe(0);
    }
}
