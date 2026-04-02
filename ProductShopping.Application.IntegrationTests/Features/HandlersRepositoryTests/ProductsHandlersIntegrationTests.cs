using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Commands.DeleteProduct;
using ProductShopping.Application.Features.Product.Commands.UpdateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProducts;
using ProductShopping.Application.IntegrationTests.Helpers;
using ProductShopping.Application.IntegrationTests.Mocks;
using ProductShopping.Persistence.Repositories;
using Shouldly;

namespace ProductShopping.Application.IntegrationTests.Features.HandlersRepositoryTests;

public class ProductsHandlersIntegrationTests
{
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProducts()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {

            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {

            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(7);
        result.Value.Metadata.TotalPages.ShouldBe(1);
        result.Value.Data.Count().ShouldBe(7);
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProductsWithCustomPagination()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 2,
                PageSize = 3
            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {
                
            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(2);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(3);
        result.Value.Metadata.TotalCount.ShouldBe(7);
        result.Value.Metadata.TotalPages.ShouldBe(3);
        result.Value.Data.Count().ShouldBe(3);
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProductsWithCustomPagination1ProductPerPageFirstPage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 1,
                PageSize = 1
            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {

            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeTrue();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(7);
        result.Value.Metadata.TotalPages.ShouldBe(7);
        result.Value.Data.Count().ShouldBe(1);
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProductsWithCustomPagination1ProductPerPageLastPage()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {
                PageNumber = 7,
                PageSize = 1
            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {

            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(7);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeTrue();
        result.Value.Metadata.PageSize.ShouldBe(1);
        result.Value.Metadata.TotalCount.ShouldBe(7);
        result.Value.Metadata.TotalPages.ShouldBe(7);
        result.Value.Data.Count().ShouldBe(1);
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProductsWithCustomFilterSearchByName()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {

            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {
                Search = "car"
            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(2);
        result.Value.Metadata.TotalPages.ShouldBe(1);
        result.Value.Data.Count().ShouldBe(2);
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProductsWithCustomFilterSearchByRatingRange()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {

            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {
                MinRating = 3.8,
                MaxRating = 4.1
            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(3);
        result.Value.Metadata.TotalPages.ShouldBe(1);
        result.Value.Data.Count().ShouldBe(3);
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_GetProductsWithCustomFilterSearchByPriceRange()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        //var result = await productsRepo.GetAsync();

        var getProductsQuery = new GetProductListQuery
        {
            PaginationParameters = new Models.Paging.PaginationParameters
            {

            },
            ProductFilterParameters = new Models.Filtering.ProductFilterParameters
            {
                MinPrice = (decimal)2,
                MaxPrice = (decimal)40
            }
        };

        var products = await productsRepo.GetAsync();
        products.Count.ShouldBe(7);

        var handler = new GetProductListQueryHandler(productsRepo);

        var result = await handler.Handle(getProductsQuery, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Metadata.CurrentPage.ShouldBe(1);
        result.Value.Metadata.HasNext.ShouldBeFalse();
        result.Value.Metadata.HasPrevious.ShouldBeFalse();
        result.Value.Metadata.PageSize.ShouldBe(10);
        result.Value.Metadata.TotalCount.ShouldBe(4);
        result.Value.Metadata.TotalPages.ShouldBe(1);
        result.Value.Data.Count().ShouldBe(4);
    }
    [Fact]
    public void ProductsHandlersIntegrationTests_AutoMapperConfiguration_IsValid()
    {
        var mapper = AutoMapperHelper.Create();
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_CreateProduct()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();

        var createProductCommand = new CreateProductCommand
        {
            Name = "Added Product",
            CategoryName = "Food",
            Price = 1,
            Rating = 1
        };

        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());
        productsRepo.GetAsync().Result.Count().ShouldBe(7);

        var handler = new CreateProductCommandHandler(productsRepo, AutoMapperHelper.Create());
        var result = await handler.Handle(createProductCommand, CancellationToken.None);

        productsRepo.GetAsync().Result.Count().ShouldBe(8);

        var product = await productsRepo.GetProductByNameAsync("Added Product");

        product.ShouldNotBeNull();
        product.Id.ShouldBe(8);
        product.CategoryId.ShouldBe(1);
        product.Price.ShouldBe(1);
        product.Rating.ShouldBe(1);
        product.Name.ShouldBe("Added Product");
        product.DateCreated.ShouldNotBeNull();
        product.DateModified.ShouldNotBeNull();
    }

    [Fact]
    public async Task ProductsHandlersIntegrationTests_UpdateProduct()
    {
        var setup = await OrdersDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());

        var updateProductCommand = new UpdateProductCommand
        {
            Id = 1,
            Name = "Gigantic Canoli",
            CategoryName = "Food",
            Price = (decimal)2.49,
            Rating = 1
        };

        var product = await productsRepo.GetByIdAsync(1);

        product.Name.ShouldBe("Little Canoli");

        var handler = new UpdateProductCommandHandler(productsRepo, AutoMapperHelper.Create());
        var result = await handler.Handle(updateProductCommand, CancellationToken.None);

        product = await productsRepo.GetByIdAsync(1);

        product.ShouldNotBeNull();
        productsRepo.GetAsync().Result.Count().ShouldBe(7);
        product.CategoryId.ShouldBe(1);
        product.Price.ShouldBe((decimal)2.49);
        product.Name.ShouldBe("Gigantic Canoli");
        product.DateCreated.ShouldNotBeNull();
        product.DateModified.ShouldNotBeNull();
    }
    [Fact]
    public async Task ProductsHandlersIntegrationTests_DeleteProduct()
    {
        var setup = await ProductsDbMocks.CreateInMemoryContextSetup();
        var productsRepo = new ProductsRepository(setup.Item1, AutoMapperHelper.Create());

        var deleteProductCommand = new DeleteProductCommand
        {
            Id = 1
        };

        var product = await productsRepo.GetByIdAsync(1);
        product.ShouldNotBeNull();

        var handler = new DeleteProductCommandHandler(productsRepo);
        var result = await handler.Handle(deleteProductCommand, CancellationToken.None);

        var getProductAgain = await productsRepo.GetByIdAsync(1);

        productsRepo.GetAsync().Result.Count().ShouldBe(6);
        getProductAgain.ShouldBeNull();
    }
}
