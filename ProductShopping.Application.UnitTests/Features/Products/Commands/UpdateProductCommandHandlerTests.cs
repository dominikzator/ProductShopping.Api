using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Commands.UpdateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.UnitTests.Mocks;
using ProductShopping.Domain.Models;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Products.Commands;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task UpdateProductWithNegativeIndex()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = -1,
            Name = "Updated Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<NotFoundException>();

        result.ErrorCode.ShouldBe(ErrorCodes.NotFound.ToString());
    }

    [Fact]
    public async Task UpdateProductWithEmptyName()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task UpdateProductWithTooLongName()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "This is a way too long name for a Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task UpdateProductWithTooLowPrice()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "Updated Product",
            CategoryName = "Food",
            Price = 0,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Price));
    }

    [Fact]
    public async Task UpdateProductWithTooLowRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "Updated Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 0.99
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task UpdateProductWithTooBigRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "Updated Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 5.01
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task UpdateProductName_WithValidationPassed()
    {
        var setup = MockProductsRepository.GetProductsRepository_UpdateProductSetup();

        var productCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "Updated Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Updated Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Updated Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new UpdateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
}
