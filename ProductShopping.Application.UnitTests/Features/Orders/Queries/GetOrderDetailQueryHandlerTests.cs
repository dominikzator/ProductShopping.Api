using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.UnitTests.Mocks;
using ProductShopping.Domain.Models;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Orders.Queries;

public class GetOrderDetailQueryHandlerTests
{
    [Fact]
    public async Task GetOrderWithInvalidId()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var ordersQuery = new GetOrderDetailQuery
        {
            Id = -1
        };

        var handler = new GetOrderDetailQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(ordersQuery, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task GetOrderWithValidId()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var productCommand = new GetOrderDetailQuery
        {
            Id = 1
        };

        var handler = new GetOrderDetailQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
