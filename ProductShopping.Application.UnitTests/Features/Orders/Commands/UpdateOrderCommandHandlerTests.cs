using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Order.Commands.UpdateOrder;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Orders.Commands;

public class UpdateOrderCommandHandlerTests
{
    [Fact]
    public async Task UpdateOrderWithInvalidId()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var updateCommand = new UpdateOrderCommand
        {
            Id = -1,
            Address = new Domain.Models.Address
            {

            }
        };

        var handler = new UpdateOrderCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(updateCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task UpdateOrderWithValidId()
    {
        // Arrange
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var updateCommand = new UpdateOrderCommand
        {
            Id = 1,
            Address = new Domain.Models.Address
            {

            }
        };

        var handler = new UpdateOrderCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(updateCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
