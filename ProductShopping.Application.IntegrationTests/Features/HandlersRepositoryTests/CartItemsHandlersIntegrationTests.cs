using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Commands.RemoveCartItem;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItems;
using ProductShopping.Application.IntegrationTests.Helpers;
using ProductShopping.Application.IntegrationTests.Mocks;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.Repositories;
using Shouldly;

namespace ProductShopping.Application.IntegrationTests.Features.HandlersRepositoryTests;

public class CartItemsHandlersIntegrationTests
{
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_GetCartItems()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var getCartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {

            }
        };

        var handler = new GetCartItemListQueryHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(getCartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Value.Data.Count().ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(1);
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_GetCartItemsCustomPagination()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var getCartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageSize = 2
            }
        };

        var handler = new GetCartItemListQueryHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(getCartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Value.Data.Count().ShouldBe(2);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(2);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(2);
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_GetCartItemsCustomPagination1PerPageFirstPage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var getCartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageSize = 1
            }
        };

        var handler = new GetCartItemListQueryHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(getCartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_GetCartItemsCustomPagination1PerPageMiddlePage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var getCartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageSize = 1,
                PageNumber = 2
            }
        };

        var handler = new GetCartItemListQueryHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(getCartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(2);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_GetCartItemsCustomPagination1PerPageLastPage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var getCartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageSize = 1,
                PageNumber = 3
            }
        };

        var handler = new GetCartItemListQueryHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(getCartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Value.Data.Count().ShouldBe(1);
        result.Value.Metadata.CurrentPage.ShouldBe(3);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_GetCartItemWithValidId()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var getCartItemQuery = new GetCartItemDetailQuery
        {
            Id = 1
        };

        var handler = new GetCartItemDetailQueryHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(getCartItemQuery, CancellationToken.None);

        result.Errors.ShouldBeEmpty();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(1);
    }

    [Fact]
    public async Task CartItemsHandlersIntegrationTests_AddCartItem()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var addCartItemCommand = new AddCartItemCommand
        {
            ProductId = 7,
            Quantity = 3
        };

        var handler = new AddCartItemCommandHandler(cartsRepo, productsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(addCartItemCommand, CancellationToken.None);

        var cartItems = await cartsRepo.GetUserCartItemsAsync("1");
        var cartItem = await cartsRepo.GetUserCartItemAsync("1", 4);

        cartItems.ShouldNotBeNull();
        cartItems.Count.ShouldBe(4);
        cartItem.ShouldNotBeNull();
        cartsRepo.GetUserCartItemsAsync("1").Result.Count().ShouldBe(4);
        cartItem.Id.ShouldBe(4);
        cartItem.CartId.ShouldBe(1);
        cartItem.ProductId.ShouldBe(7);
        cartItem.Quantity.ShouldBe(3);
        cartItem.DateCreated.ShouldNotBeNull();
        cartItem.DateModified.ShouldNotBeNull();
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_Add2DifferentCartItems()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var addFirstCartItemCommand = new AddCartItemCommand
        {
            ProductId = 6,
            Quantity = 3
        };

        var addSecondCartItemCommand = new AddCartItemCommand
        {
            ProductId = 7,
            Quantity = 2
        };

        var handler = new AddCartItemCommandHandler(cartsRepo, productsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result1 = await handler.Handle(addFirstCartItemCommand, CancellationToken.None);
        var result2 = await handler.Handle(addSecondCartItemCommand, CancellationToken.None);

        var cartItems = await cartsRepo.GetUserCartItemsAsync("1");

        cartItems.ShouldNotBeNull();
        cartItems.Count().ShouldBe(5);
        cartItems[3].Id.ShouldBe(4);
        cartItems[3].ProductId.ShouldBe(6);
        cartItems[3].Quantity.ShouldBe(3);
        cartItems[3].DateCreated.ShouldNotBeNull();
        cartItems[3].DateModified.ShouldNotBeNull();
        cartItems[4].Id.ShouldBe(5);
        cartItems[4].ProductId.ShouldBe(7);
        cartItems[4].Quantity.ShouldBe(2);
        cartItems[4].DateCreated.ShouldNotBeNull();
        cartItems[4].DateModified.ShouldNotBeNull();
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_Add2TheSameCartItems()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var addFirstCartItemCommand = new AddCartItemCommand
        {
            ProductId = 7,
            Quantity = 3
        };

        var addSecondCartItemCommand = new AddCartItemCommand
        {
            ProductId = 7,
            Quantity = 2
        };

        var handler = new AddCartItemCommandHandler(cartsRepo, productsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result1 = await handler.Handle(addFirstCartItemCommand, CancellationToken.None);
        var result2 = await handler.Handle(addSecondCartItemCommand, CancellationToken.None);

        var cartItems = await cartsRepo.GetUserCartItemsAsync("1");

        cartItems.ShouldNotBeNull();
        cartItems.Count().ShouldBe(4);
        cartItems[3].Id.ShouldBe(4);
        cartItems[3].ProductId.ShouldBe(7);
        cartItems[3].Quantity.ShouldBe(5);
        cartItems[3].DateCreated.ShouldNotBeNull();
        cartItems[3].DateModified.ShouldNotBeNull();
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_RemoveCartItemWithLessQuantity()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var cartItems = await cartsRepo.GetUserCartItemsAsync("1");
        cartItems.Count.ShouldBe(3);

        var cartItem = await cartsRepo.GetUserCartItemAsync("1", 3);
        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(3);

        var removeCartItemCommand = new RemoveCartItemCommand
        {
            CartItemId = 3,
            Quantity = 1
        };

        var handler = new RemoveCartItemCommandHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(removeCartItemCommand, CancellationToken.None);

        cartItem = await cartsRepo.GetUserCartItemAsync("1", 3);
        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(2);

        cartItems = await cartsRepo.GetUserCartItemsAsync("1");
        cartItems.Count.ShouldBe(3);
    }
    [Fact]
    public async Task CartItemsHandlersIntegrationTests_RemoveWholeCartItemWithAllQuantity()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var cartsRepo = new CartsRepository(setup.Item1, AutoMapperHelper.Create());
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock.Setup(r => r.GetUserId()).Returns("1");

        var cartItems = await cartsRepo.GetUserCartItemsAsync("1");
        cartItems.Count.ShouldBe(3);

        var cartItem = await cartsRepo.GetUserCartItemAsync("1", 3);
        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(3);

        var removeCartItemCommand = new RemoveCartItemCommand
        {
            CartItemId = 3,
            Quantity = 3
        };

        var handler = new RemoveCartItemCommandHandler(cartsRepo, usersServiceMock.Object, AutoMapperHelper.Create());
        var result = await handler.Handle(removeCartItemCommand, CancellationToken.None);

        cartItem = await cartsRepo.GetUserCartItemAsync("1", 3);
        cartItem.ShouldBeNull();

        cartItems = await cartsRepo.GetUserCartItemsAsync("1");
        cartItems.Count.ShouldBe(2);
    }
}
