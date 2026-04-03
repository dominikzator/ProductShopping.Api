using EmptyFiles;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Api.EndToEndTests.Seeders;

public static class TestDataSeeder
{
    public static async Task Seed(ProductShoppingDbContext context)
    {
        if (context.Products.Any())
        {
            return;
        }

        var products = new[]
{
            new Product
            {
                Id = 1,
                Name = "Little Canoli",
                CategoryId = 1,
                Price = (decimal)2.49,
                Rating = 3.5
            },
            new Product
            {
                Id = 2,
                Name = "Car Cleaner",
                CategoryId = 3,
                Price = (decimal)49.99,
                Rating = 3.8
            },
            new Product
            {
                Id = 3,
                Name = "Car Wheel",
                CategoryId = 3,
                Price = (decimal)39.99,
                Rating = 3.2
            },
            new Product
            {
                Id = 4,
                Name = "Bananas",
                CategoryId = 1,
                Price = (decimal)2.99,
                Rating = 4.3
            },
            new Product
            {
                Id = 5,
                Name = "Milk",
                CategoryId = 1,
                Price = (decimal)1.99,
                Rating = 4.8
            },
            new Product
            {
                Id = 6,
                Name = "Vacuum Cleaner",
                CategoryId = 3,
                Price = (decimal)69.99,
                Rating = 4
            },
            new Product
            {
                Id = 7,
                Name = "Birthday Cake",
                CategoryId = 1,
                Price = (decimal)29.99,
                Rating = 3.9
            }
        }.ToList();

        var productsDtos = new[]
        {
            new ProductDto
            {
                Id = 1,
                Name = "Little Canoli",
                CategoryName = "Food",
                Price = (decimal)2.49,
                Rating = 3.5
            },
            new ProductDto
            {
                Id = 2,
                Name = "Car Cleaner",
                CategoryName = "Automotive",
                Price = (decimal)49.99,
                Rating = 3.8
            },
            new ProductDto
            {
                Id = 3,
                Name = "Car Wheel",
                CategoryName = "Automotive",
                Price = (decimal)39.99,
                Rating = 3.2
            },
            new ProductDto
            {
                Id = 4,
                Name = "Bananas",
                CategoryName = "Food",
                Price = (decimal)2.99,
                Rating = 4.3
            },
            new ProductDto
            {
                Id = 5,
                Name = "Milk",
                CategoryName = "Food",
                Price = (decimal)1.99,
                Rating = 4.8
            },
            new ProductDto
            {
                Id = 6,
                Name = "Vacuum Cleaner",
                CategoryName = "Automotive",
                Price = (decimal)69.99,
                Rating = 4
            },
            new ProductDto
            {
                Id = 7,
                Name = "Birthday Cake",
                CategoryName = "Food",
                Price = (decimal)29.99,
                Rating = 3.9
            },
        }.ToList();

        var cart = new Cart
        {
            Id = 1,
            UserId = "1"
        };

        var cartItems = new CartItem[]
        {
            new CartItem
            {
                Id = 1,
                CartId = 1,
                ProductId = 1,
                Quantity = 1,

            },
            new CartItem
            {
                Id = 2,
                CartId = 1,
                ProductId = 2,
                Quantity = 2
            },
             new CartItem
            {
                Id = 3,
                CartId = 1,
                ProductId = 3,
                Quantity = 3
            },
        }.ToList();

        var orderItems = new OrderItem[]
        {
            new OrderItem
            {
                Id = 1,
                CustomerId = "1",
                OrderId = 1,
                ProductId = 1,
                Quantity = 1,
            },
            new OrderItem
            {
                Id = 2,
                CustomerId = "1",
                OrderId = 2,
                ProductId = 2,
                Quantity = 2,
            },
            new OrderItem
            {
                Id = 3,
                CustomerId = "1",
                OrderId = 3,
                ProductId = 3,
                Quantity = 3,
            },
        }.ToList();

        var orders = new Order[]
        {
            new Order
            {
                Id = 1,
                OrderNumber = "1",
                CustomerId = "1",
                Address = new Address
                {

                }
            },
            new Order
            {
                Id = 2,
                OrderNumber = "2",
                CustomerId = "1",
                Address = new Address
                {

                }
            },
            new Order
            {
                Id = 3,
                OrderNumber = "3",
                CustomerId = "1",
                Address = new Address
                {

                }
            }
        }.ToList();

        await context.Products.AddRangeAsync(products);
        await context.Carts.AddAsync(cart);
        await context.CartItems.AddRangeAsync(cartItems);
        await context.Orders.AddRangeAsync(orders);
        await context.OrderItems.AddRangeAsync(orderItems);
        await context.SaveChangesAsync();
    }
}