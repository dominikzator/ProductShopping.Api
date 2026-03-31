using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.CartItems.Queries;

public class GetCartItemDetailQueryHandlerTests
{
    [Fact]
    public async Task GetCartItemWithInvalidId()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartQuery = new GetCartItemDetailQuery
        {
            Id = -1
        };

        var handler = new GetCartItemDetailQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartQuery, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task GetCartItemWithExistingId()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartQuery = new GetCartItemDetailQuery
        {
            Id = 1
        };
        var handler = new GetCartItemDetailQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
