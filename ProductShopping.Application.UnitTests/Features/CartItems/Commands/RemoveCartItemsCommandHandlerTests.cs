using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItems;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.CartItems.Commands;

public class RemoveCartItemsCommandHandlerTests
{
    [Fact]
    public async Task RemoveCartItemsValid()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new RemoveCartItemsCommand
        {

        };

        var handler = new RemoveCartItemsCommandHandler(setup.Item1.Object, setup.Item3.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.Count().ShouldBe(0);
    }
}
