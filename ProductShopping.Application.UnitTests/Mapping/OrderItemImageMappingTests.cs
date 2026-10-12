using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.MappingProfiles;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.UnitTests.Mapping;

public class OrderItemImageMappingTests
{
    private static IMapper CreateMapper()
    {
        var expression = new MapperConfigurationExpression();
        expression.AddProfile<ProductMappingProfile>();
        var configuration = new MapperConfiguration(expression, LoggerFactory.Create(_ => { }));
        return configuration.CreateMapper();
    }

    [Fact]
    public void MapsProductImageWithoutChangingOrderSnapshot()
    {
        var item = new OrderItem
        {
            ProductName = "Name at checkout",
            UnitPrice = 34.99m,
            Product = new Product
            {
                Name = "Updated product name",
                Price = 99m,
                ImageUrl = "https://example.com/products/board-game.png"
            }
        };

        var dto = CreateMapper().Map<OrderItemDto>(item);

        Assert.Equal(item.Product.ImageUrl, dto.ImageUrl);
        Assert.Equal("Name at checkout", dto.ProductName);
        Assert.Equal(34.99m, dto.UnitPrice);
    }

    [Fact]
    public void MissingProductDoesNotBreakMapping()
    {
        var dto = CreateMapper().Map<OrderItemDto>(new OrderItem { Product = null! });

        Assert.Null(dto.ImageUrl);
    }

    [Fact]
    public void ReverseMappingDoesNotCreateProductFromImageUrl()
    {
        var item = CreateMapper().Map<OrderItem>(new OrderItemDto
        {
            ProductId = 7,
            ImageUrl = "https://example.com/untrusted-image.png"
        });

        Assert.Equal(7, item.ProductId);
        Assert.Null(item.Product);
    }
}
