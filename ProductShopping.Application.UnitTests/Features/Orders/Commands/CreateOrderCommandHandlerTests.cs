using Microsoft.Extensions.Configuration;
using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.UnitTests.Mocks;
using ProductShopping.Domain.Models;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task CreateOrderWithSuccessValidation()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }
    [Fact]
    public async Task CreateOrderWithTooLongStreetAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                Street = "This is a definitely a way too long Name for a Street Address"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.Street)}");
    }
    [Fact]
    public async Task CreateOrderWithEmptyBuildingNumberAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                BuildingNumber = ""
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.BuildingNumber)}");
    }
    [Fact]
    public async Task CreateOrderWithTooLongBuildingNumberAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                BuildingNumber = "98765"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.BuildingNumber)}");
    }
    [Fact]
    public async Task CreateOrderWithTooLongApartmentNumberAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                ApartmentNumber = "9876"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.ApartmentNumber)}");
    }
    [Fact]
    public async Task CreateOrderWithEmptyCityAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                City = ""
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.City)}");
    }
    [Fact]
    public async Task CreateOrderWithTooLongCityAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                City = "This is a way too long name for a city, it is so long that it would be a problem to display it on a page in some container"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.City)}");
    }
    [Fact]
    public async Task CreateOrderWithEmptyPostalCodeAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                PostalCode = ""
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.PostalCode)}");
    }
    [Fact]
    public async Task CreateOrderWithTooLongPostalCodeAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                PostalCode = "This is a way too long name for a PostalCode, it is so long that it would be a problem to display it on a page in some container"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.PostalCode)}");
    }
    [Fact]
    public async Task CreateOrderWithEmptyCountryAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                Country = ""
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.Country)}");
    }
    [Fact]
    public async Task CreateOrderWithTooLongCountryAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                Country = "This is a way too long name for a Country, it is so long that it would be a problem to display it on a page in some container"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.Country)}");
    }
    [Fact]
    public async Task CreateOrderWithEmptyPhoneNumberAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                PhoneNumber = ""
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.PhoneNumber)}");
    }
    [Fact]
    public async Task CreateOrderWithTooLongPhoneNumberAddress()
    {
        var setup = MockOrdersRepository.GetOrdersRepository_OrdersSetup();

        var orderCommand = new CreateOrderCommand
        {
            Address = new Address
            {
                PhoneNumber = "This is a way too long name for a PhoneNumber, it is so long that it would be a problem to display it on a page in some container"
            }
        };

        var paymentsMock = new Mock<IPaymentsService>();
        var configMock = new Mock<IConfiguration>();

        var handler = new CreateOrderCommandHandler(setup.Item1.Object, setup.Item4.Object, setup.Item2.Object, paymentsMock.Object, configMock.Object, setup.Item3.Object);

        var result = await handler.Handle(orderCommand, CancellationToken.None).ShouldThrowAsync<ValidationFailedException>();

        result.ErrorCode.ShouldBe(ErrorCodes.Validation.ToString());
        result.InvalidFieldName.ShouldBe($"{nameof(orderCommand.Address)}.{nameof(orderCommand.Address.PhoneNumber)}");
    }
}
