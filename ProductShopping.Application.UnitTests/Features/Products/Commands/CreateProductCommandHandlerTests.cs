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

namespace ProductShopping.Application.UnitTests.Features.Products.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task CreateProductWithInvalidValidationNameEmpty()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task CreateProductWithTooLongName()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "This is a definitely way too long name for a Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task CreateProductWithDuplicateName()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "Test Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Name));
    }

    [Fact]
    public async Task CreateProductWithEmptyCategory()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.CategoryName));
    }

    [Fact]
    public async Task CreateProductWithCategoryThatDoesntExist()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Foodsy",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.CategoryName));
    }

    [Fact]
    public async Task CreateProductWithNegativeRating()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = -4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task CreateProductWithTooLessRating()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 0.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task CreateProductWithTooMuchRating()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 5.1
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Rating));
    }

    [Fact]
    public async Task CreateProductWithNegativePrice()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = -1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Price));
    }

    [Fact]
    public async Task CreateProductWithZeroPrice()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 0,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe(nameof(dto.Price));
    }

    [Fact]
    public async Task CreateProductWithMinimalRating()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 1
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }

    [Fact]
    public async Task CreateProductWithMaximalRating()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 5
        };
        var product = new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 1, Rating = 4.5 };
        var dto = new ProductDto { Id = 1, Name = "Test Product", CategoryName = "Food", Price = 1, Rating = 4.5 };

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }

    [Fact]
    public async Task CreateProductWithSuccessValidation()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_CreateProduct();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<CreateProductCommandHandler>>();

        var productCommand = new CreateProductCommand
        {
            Name = "A new product Name",
            CategoryName = "Food",
            Price = 1,
            Rating = 4.5
        };
        var product = new Product { Id = 1, Name = "A new product Name", CategoryId = 1, Price = 1, Rating = 4.5};
        var dto = new ProductDto { Id = 1, Name = "A new product Namet", CategoryName = "Food", Price = 1, Rating = 4.5};

        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        mapperMock.Setup(m => m.Map<Product>(productCommand))
                  .Returns(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product))
                  .Returns(dto);

        var handler = new CreateProductCommandHandler(repoMock.Object, mapperMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }
}