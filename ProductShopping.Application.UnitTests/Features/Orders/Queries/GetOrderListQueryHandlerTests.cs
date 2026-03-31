using ProductShopping.Application.Features.CartItem.Queries.GetCartItems;
using ProductShopping.Application.Features.Order.Queries.GetOrders;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Orders.Queries;

public class GetOrderListQueryHandlerTests
{
    [Fact]
    public async Task GetAllUserOrdersNoFilters()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new PaginationParameters
            {

            }
        };

        var handler = new GetOrderListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(3);
        result.Value.Metadata.PageSize.ShouldBe(10);
    }

    [Fact]
    public async Task GetAllUserOrdersWithCustomPagination()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 1,
                PageSize = 10
            }
        };

        var handler = new GetOrderListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(3);
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(1);
    }
    [Fact]
    public async Task GetAllUserOrdersWithCustomPagination1OrderPerPageFirstPage()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 1,
                PageSize = 1
            }
        };
        var handler = new GetOrderListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.HasNext.ShouldBeTrue();
    }
    [Fact]
    public async Task GetAllUserOrdersWithCustomPagination1OrderPerPageSecondPage()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 2,
                PageSize = 1
            }
        };
        var handler = new GetOrderListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(2);
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.HasNext.ShouldBeTrue();
    }
    [Fact]
    public async Task GetAllOrdersWithCustomPagination1ItemPerPageLastPage()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var ordersQuery = new GetOrderListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 3,
                PageSize = 1
            }
        };
        var handler = new GetOrderListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(ordersQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(3);
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.HasNext.ShouldBeFalse();
    }
}
