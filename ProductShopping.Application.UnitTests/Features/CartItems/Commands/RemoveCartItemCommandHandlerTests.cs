using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.CartItems.Commands;

public class RemoveCartItemCommandHandlerTests
{
    [Fact]
    public async Task RemoveCartItemWithInvalidId()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new RemoveCartItemCommand
        {
            CartItemId = -1,
            Quantity = 1
        };

        var handler = new RemoveCartItemCommandHandler(setup.Item1.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }
    [Fact]
    public async Task RemoveCartItemWithLessQuantity()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new RemoveCartItemCommand
        {
            CartItemId = 1,
            Quantity = 1
        };

        var handler = new RemoveCartItemCommandHandler(setup.Item1.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.Count().ShouldBe(0);
    }
    [Fact]
    public async Task RemoveCartItemWithEqualQuantity()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new RemoveCartItemCommand
        {
            CartItemId = 1,
            Quantity = 3
        };

        var handler = new RemoveCartItemCommandHandler(setup.Item1.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.Count().ShouldBe(0);
    }
    [Fact]
    public async Task RemoveCartItemWithTooMuchQuantity()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new RemoveCartItemCommand
        {
            CartItemId = 1,
            Quantity = 4
        };

        var handler = new RemoveCartItemCommandHandler(setup.Item1.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None).ShouldThrowAsync<BadRequestException>();

        result.ErrorCode.ShouldBe(ErrorCodes.BadRequest.ToString());
    }
    [Fact]
    public async Task RemoveCartItemWithValidId()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new RemoveCartItemCommand
        {
            CartItemId = 1,
            Quantity = 1
        };

        var handler = new RemoveCartItemCommandHandler(setup.Item1.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.Count().ShouldBe(0);
    }
}
