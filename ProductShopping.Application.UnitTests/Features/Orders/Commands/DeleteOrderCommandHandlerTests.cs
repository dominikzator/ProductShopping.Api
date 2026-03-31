using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Order.Commands.DeleteOrder;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Orders.Commands;

public class DeleteOrderCommandHandlerTests
{
    [Fact]
    public async Task DeleteOrderWithInvalidId()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var deleteCommand = new DeleteOrderCommand
        {
            Id = -1
        };

        var handler = new DeleteOrderCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(deleteCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task DeleteOrderWithValidId()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var deleteCommand = new DeleteOrderCommand
        {
            Id = 1
        };

        var handler = new DeleteOrderCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(deleteCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
