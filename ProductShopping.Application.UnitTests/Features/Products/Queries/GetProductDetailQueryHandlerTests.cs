using AutoMapper;
using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
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
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductDetailQuery
        {
            Id = -1
        };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var handler = new GetProductDetailQueryHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task GetProductWithTooBigId()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductDetailQuery
        {
            Id = 999
        };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var handler = new GetProductDetailQueryHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task GetProductWithExistingId()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductDetailQuery
        {
            Id = 1
        };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var handler = new GetProductDetailQueryHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
