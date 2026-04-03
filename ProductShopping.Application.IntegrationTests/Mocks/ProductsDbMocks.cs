using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Application.IntegrationTests.Mocks;

public class ProductsDbMocks
{
    public async static Task<(ProductShoppingDbContext, Mock<IMapper>)> CreateInMemoryContextSetup()
    {
        var options = new DbContextOptionsBuilder<ProductShoppingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // osobna baza per test
            .Options;

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new ProductShoppingDbContext(options, httpContextAccessor.Object);
        var mapperMock = new Mock<IMapper>();

        var products = new[]
        {
            new Product
            {
                Id = 1,
                Name = "Little Canoli",
                CategoryId = 1,
                Price = (decimal)2.49
            },
            new Product
            {
                Id = 2,
                Name = "Car Cleaner",
                CategoryId = 3,
                Price = (decimal)49.99
            },
            new Product
            {
                Id = 3,
                Name = "Car Wheel",
                CategoryId = 3,
                Price = (decimal)39.99
            },
            new Product
            {
                Id = 4,
                Name = "Bananas",
                CategoryId = 1,
                Price = (decimal)2.99
            },
            new Product
            {
                Id = 5,
                Name = "Milk",
                CategoryId = 1,
                Price = (decimal)1.99
            },
            new Product
            {
                Id = 6,
                Name = "Vacuum Cleaner",
                CategoryId = 3,
                Price = (decimal)69.99
            },
            new Product
            {
                Id = 7,
                Name = "Birthday Cake",
                CategoryId = 1,
                Price = (decimal)29.99
            },
        }.ToList();

        var productsDtos = new[]
{
            new ProductDto
            {
                Id = 1,
                Name = "Little Canoli",
                CategoryName = "Food",
                Price = (decimal)2.49
            },
            new ProductDto
            {
                Id = 2,
                Name = "Car Cleaner",
                CategoryName = "Automotive",
                Price = (decimal)49.99
            },
            new ProductDto
            {
                Id = 3,
                Name = "Car Wheel",
                CategoryName = "Automotive",
                Price = (decimal)39.99
            },
            new ProductDto
            {
                Id = 4,
                Name = "Bananas",
                CategoryName = "Food",
                Price = (decimal)2.99
            },
            new ProductDto
            {
                Id = 5,
                Name = "Milk",
                CategoryName = "Food",
                Price = (decimal)1.99
            },
            new ProductDto
            {
                Id = 6,
                Name = "Vacuum Cleaner",
                CategoryName = "Automotive",
                Price = (decimal)69.99
            },
            new ProductDto
            {
                Id = 7,
                Name = "Birthday Cake",
                CategoryName = "Food",
                Price = (decimal)29.99
            },
        }.ToList();

        var foodCategory = new ProductCategory
        {
            Id = 1,
            Name = "Food"
        };
        var automotiveCategory = new ProductCategory
        {
            Id = 3,
            Name = "Automotive"
        };

        mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>()))
        .Returns((ProductDto productDto) =>
        {
            return products.FirstOrDefault(p => p.Id == productDto.Id);
        });
        mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
        .Returns((Product product) =>
        {
            return productsDtos.FirstOrDefault(p => p.Id == product.Id);
        });

        await context.ProductCategories.AddAsync(foodCategory);
        await context.ProductCategories.AddAsync(automotiveCategory);
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return (context, mapperMock);
    }
}
