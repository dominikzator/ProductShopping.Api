using ProductShopping.Application.Features.CartItem.Queries.GetCartItems;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.CartItems.Queries;

public class GetCartItemListQueryHandlerTests
{
    [Fact]
    public async Task GetAllUserCartItemsNoFilters()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new PaginationParameters
            {

            }
        };

        var handler = new GetCartItemListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(3);
        result.Value.Metadata.PageSize.ShouldBe(10);
    }

    [Fact]
    public async Task GetAllUserCartItemsWithCustomPagination()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageSize = 1
            }
        };
        var handler = new GetCartItemListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
    }
    [Fact]
    public async Task GetAllUserCartItemsWithCustomPagination1ItemPerPageFirstPage()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 1,
                PageSize = 1
            }
        };
        var handler = new GetCartItemListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.HasNext.ShouldBeTrue();
    }
    public async Task GetAllUserCartItemsWithCustomPagination1ItemPerPageSecondPage()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 2,
                PageSize = 1
            }
        };
        var handler = new GetCartItemListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.HasNext.ShouldBeTrue();
    }
    public async Task GetAllUserCartItemsWithCustomPagination1ItemPerPageLastPage()
    {
        // Arrange
        var setup = MockCartsRepository.GetCartsRepository_GetCartItemsSetup();

        var cartItemsQuery = new GetCartItemListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageNumber = 3,
                PageSize = 1
            }
        };

        var handler = new GetCartItemListQueryHandler(setup.Item1.Object, setup.Item2.Object, setup.Item3.Object);

        var result = await handler.Handle(cartItemsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.HasNext.ShouldBeFalse();
    }
}
