using AutoMapper;
using Moq;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Exceptions;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Features.Product.Queries.GetProducts;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.UnitTests.Mocks;
using Shouldly;

namespace ProductShopping.Application.UnitTests.Features.Products.Queries;

public class GetProductListQueryHandlerTests
{
    [Fact]
    public async Task GetAllProductsNoFilters()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                
            },
            ProductFilterParameters = new ProductFilterParameters
            {
                
            }
        };

        var handler = new GetProductListQueryHandler(repoMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(7);
        result.Value.Metadata.PageSize.ShouldBe(10);
    }
    [Fact]
    public async Task GetAllProductsWithCategoryFilter()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                
            },
            ProductFilterParameters = new ProductFilterParameters
            {
                CategoryName = "Food"
            }
        };

        repoMock.Setup(r => r.GetFilteredRawPagedAsync(It.IsAny<ProductFilterParameters>(), It.IsAny<PaginationParameters>()))
            .ReturnsAsync((ProductFilterParameters productFilterParameters, PaginationParameters paginationParameters) => {
                return ([
                    new ProductDto { Name = "First"},
                    new ProductDto { Name = "Second"},
                    new ProductDto { Name = "Third"},
                    new ProductDto { Name = "Fourth"},
                ], 4, 1);
            });

        var handler = new GetProductListQueryHandler(repoMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(4);
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalPages.ShouldBe(1);
    }
    [Fact]
    public async Task GetAllProductsWithCustomPagination()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductListQuery
        {
            PaginationParameters = new PaginationParameters
            {
                PageSize = 1
            },
            ProductFilterParameters = new ProductFilterParameters
            {

            }
        };

        repoMock.Setup(r => r.GetFilteredRawPagedAsync(It.IsAny<ProductFilterParameters>(), It.IsAny<PaginationParameters>()))
            .ReturnsAsync((ProductFilterParameters productFilterParameters, PaginationParameters paginationParameters) => {
                return ([
                    new ProductDto { Name = "First"},
                ], 4, 4);
            });

        var handler = new GetProductListQueryHandler(repoMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(1);
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(4);
        result.Value.Metadata.TotalPages.ShouldBe(4);
    }
    [Fact]
    public async Task GetEmptyProducts()
    {
        // Arrange
        var repoMock = MockProductsRepository.GetProductsRepository_GetProducts();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetProductDetailQueryHandler>>();

        var productCommand = new GetProductListQuery
        {
            PaginationParameters = new PaginationParameters
            {

            },
            ProductFilterParameters = new ProductFilterParameters
            {
                Search = "This filter is too complicated to find at least 1 matching Product"
            }
        };

        repoMock.Setup(r => r.GetFilteredRawPagedAsync(It.IsAny<ProductFilterParameters>(), It.IsAny<PaginationParameters>()))
            .ReturnsAsync((ProductFilterParameters productFilterParameters, PaginationParameters paginationParameters) => {
                return ([], 0, 0);
            });

        var handler = new GetProductListQueryHandler(repoMock.Object);

        var result = await handler.Handle(productCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Data.Count().ShouldBe(0);
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(0);
        result.Value.Metadata.TotalPages.ShouldBe(0);
    }
}
