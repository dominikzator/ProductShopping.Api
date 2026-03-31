using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.UnitTests.Mocks;
using ProductShopping.Domain.Models;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Products.Queries;

public class GetProductDetailQueryHandlerTests
{
    [Fact]
    public async Task GetProductWithNegativeId()
    {
        // Arrange
        var setup = MockProductsRepository.GetProductsRepository_GetProductsSetup();

        var productCommand = new GetProductDetailQuery
        {
            Id = -1
        };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var handler = new GetProductDetailQueryHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task GetProductWithTooBigId()
    {
        // Arrange
        var setup = MockProductsRepository.GetProductsRepository_GetProductsSetup();

        var productCommand = new GetProductDetailQuery
        {
            Id = 999
        };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var handler = new GetProductDetailQueryHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task GetProductWithExistingId()
    {
        // Arrange
        var setup = MockProductsRepository.GetProductsRepository_GetProductsSetup();

        var productCommand = new GetProductDetailQuery
        {
            Id = 1
        };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var handler = new GetProductDetailQueryHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
