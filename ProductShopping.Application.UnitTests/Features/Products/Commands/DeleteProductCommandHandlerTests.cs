using AutoMapper;
using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Commands.DeleteProduct;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Products.Commands;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task DeleteProductWithInvalidId()
    {
        var repoMock = MockProductsRepository.GetProductsRepository_UpdateProduct();
        var mapperMock = new Mock<IMapper>();

        var productCommand = new DeleteProductCommand
        {
            Id = -1
        };

        var handler = new DeleteProductCommandHandler(repoMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task DeleteValidProduct()
    {
        var repoMock = MockProductsRepository.GetProductsRepository_UpdateProduct();
        var mapperMock = new Mock<IMapper>();

        var productCommand = new DeleteProductCommand
        {
            Id = 1
        };

        var handler = new DeleteProductCommandHandler(repoMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
