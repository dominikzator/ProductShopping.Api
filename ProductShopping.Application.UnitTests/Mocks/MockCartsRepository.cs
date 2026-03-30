using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.UnitTests.Mocks
{
    public class MockCartsRepository
    {
        public static Mock<ICartsRepository> GetCartsRepository_GetCartItems()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    CartId = 1,
                    ProductId = 3333,
                    Quantity = 3
                },
                new CartItem
                {
                    CartId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new CartItem
                {
                    CartId = 1,
                    ProductId = 3335,
                    Quantity = 5
                }
            };

            var cartItemsDtos = new List<CartItemDto>
            {
                new CartItemDto
                {
                    Id = 1,
                    CartId = 1,
                    ProductId = 3333,
                    Quantity = 3
                },
                new CartItemDto
                {
                    CartId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new CartItemDto
                {
                    CartId = 1,
                    ProductId = 3335,
                    Quantity = 5
                }
            };

            var cart = new Cart
            {
                Id = 1,
                UserId = "1",
                CartItems = cartItems
            };

            var cartDto = new CartDto
            {
                Id = 1,
                UserId = "1",
                CartItems = cartItemsDtos
            };

            var mockRepo = new Mock<ICartsRepository>();
            var mockUsersService = new Mock<IUsersService>();

            mockRepo.Setup(r => r.GetUserCartAsync(It.IsAny<string>())).Returns((string userId) => cartDto);
            mockUsersService.Setup(r => r.GetUserId()).Returns("1");

            return mockRepo;
        }
    }
}
