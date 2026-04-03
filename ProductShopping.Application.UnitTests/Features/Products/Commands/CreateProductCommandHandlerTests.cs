using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.UnitTests.Mocks;
using ProductShopping.Domain.Models;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Products.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task CreateProductWithInvalidValidationNameEmpty()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task CreateProductWithTooLongName()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "This is a definitely way too long name for a Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task CreateProductWithDuplicateName()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "Test Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task CreateProductWithEmptyCategory()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.CategoryName));
    }

    [Fact]
    public async Task CreateProductWithCategoryThatDoesntExist()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Foodsy",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.CategoryName));
    }

    [Fact]
    public async Task CreateProductWithNegativeRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = -4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task CreateProductWithTooLessRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 0.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task CreateProductWithTooMuchRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 5.1
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task CreateProductWithNegativePrice()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = -1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Price));
    }

    [Fact]
    public async Task CreateProductWithZeroPrice()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 0,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Price));
    }

    [Fact]
    public async Task CreateProductWithMinimalRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 1
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }

    [Fact]
    public async Task CreateProductWithMaximalRating()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }

    [Fact]
    public async Task CreateProductWithSuccessValidation()
    {
        var setup = MockProductsRepository.GetProductsRepository_CreateProductSetup();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "A new product Name", CategoryId = 1, Price = 1, Rating = 4.5};
        var dto = new ProductDto { Id = 1, Name = "A new product Name", CategoryName = "Food", Price = 1, Rating = 4.5};

        setup.Item1
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        setup.Item2.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        setup.Item2.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        //for success validation with checking duplicate name
        setup.Item1.Setup(r => r.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((string productName) =>
        {
            return null;
        });

        var handler = new CreateProductCommandHandler(setup.Item1.Object, setup.Item2.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }
}