using AutoMapper;
using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.CartItems.Queries;

public class GetCartItemDetailRequestHandlerTests
{
    [Fact]
    public async Task GetCartItemWithExistingId()
    {
        // Arrange
        var repoMock = MockCartsRepository.GetCartsRepository_GetCartItems();
        var mapperMock = new Mock<IMapper>();
        var usersServiceMock = new Mock<IUsersService>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var cartsCommand = new GetCartItemDetailQuery
        {
            Id = 1
        };
        var handler = new GetCartItemDetailQueryHandler(repoMock.Object, usersServiceMock.Object, mapperMock.Object);

        var result = await handler.Handle(cartsCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
