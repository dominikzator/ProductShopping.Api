using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.CartItems.Commands;

public class AddCartItemCommandHandlerTests
{
    [Fact]
    public async Task AddCartItemWithInvalidId()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new AddCartItemCommand
        {
            ProductId = -1,
            Quantity = 1
        };

        var handler = new AddCartItemCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task AddCartItemWithNegativeQuantity()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new AddCartItemCommand
        {
            ProductId = 3333,
            Quantity = -1
        };

        var handler = new AddCartItemCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(cartsCommand.Quantity));
    }

    [Fact]
    public async Task AddCartItemWithValidId()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_AddCartItemSetup();

        var cartsCommand = new AddCartItemCommand
        {
            ProductId = 3333,
            Quantity = 1
        };

        var handler = new AddCartItemCommandHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object, setup.Item4.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.Count().ShouldBe(0);
        result.Value!.ProductId.ShouldBe(3333);
    }
}
