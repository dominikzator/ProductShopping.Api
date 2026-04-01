using AutoMapper;
using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.UnitTests.Mocks
{
    public class MockCartsRepository
    {
        public static (Mock<ICartsRepository>, Mock<IUsersService>, Mock<IMapper>) GetCartsRepository_GetCartItemsSetup()
        {
            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    CartId = 1,
                    ProductId = 3333,
                    Quantity = 3
                },
                new CartItem
                {
                    Id = 2,
                    CartId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new CartItem
                {
                    Id = 3,
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
                    Id = 2,
                    CartId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new CartItemDto
                {
                    Id = 3,
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
            var mapperMock = new Mock<IMapper>();

            mockRepo.Setup(r => r.GetUserCartDtoAsync(It.IsAny<string>())).ReturnsAsync((string userId) => Result<CartDto>.Success(cartDto));
            mockRepo.Setup(r => r.GetUserCartItemDtoAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((string userId, int cartItemId) =>
            {
                var cartItemDto = cartItemsDtos.FirstOrDefault(p => p.Id == cartItemId);
                return (cartItemDto == null) ? Result<CartItemDto>.Failure() : Result<CartItemDto>.Success(cartItemDto);
            });
            mockRepo.Setup(r => r.GetUserCartItemsDtosAsync(It.IsAny<string>())).ReturnsAsync((string userId) =>
            {
                return Result<List<CartItemDto>>.Success(cartItemsDtos);
            });
            mockUsersService.Setup(r => r.GetUserId()).Returns("1");

            mapperMock.Setup(m => m.Map<List<CartItemDto>>(cartItems))
          .Returns(cartItemsDtos);

            mapperMock.Setup(m => m.Map<List<CartItem>>(cartItemsDtos))
                      .Returns(cartItems);

            return (mockRepo, mockUsersService, mapperMock);
        }

        public static (Mock<ICartsRepository>, Mock<IProductsRepository>, Mock<IUsersService>, Mock<IMapper>) GetCartsRepository_AddCartItemSetup()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 3333,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 3334,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 3335,
                    CategoryId = 1,
                },
            };
            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    CartId = 1,
                    ProductId = 3333,
                    Quantity = 3,
                },
                new CartItem
                {
                    Id = 2,
                    CartId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new CartItem
                {
                    Id = 3,
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
                    Id = 2,
                    CartId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new CartItemDto
                {
                    Id = 3,
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

            var cartsRepo = new Mock<ICartsRepository>();
            var productsRepo = new Mock<IProductsRepository>();
            var mockUsersService = new Mock<IUsersService>();
            var mapperMock = new Mock<IMapper>();

            cartsRepo.Setup(r => r.GetUserCartDtoAsync(It.IsAny<string>())).ReturnsAsync((string userId) => Result<CartDto>.Success(cartDto));
            cartsRepo.Setup(r => r.GetUserCartItemDtoAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((string userId, int cartItemId) =>
            {
                var cartItemDto = cartItemsDtos.FirstOrDefault(p => p.Id == cartItemId);
                return (cartItemDto == null) ? Result<CartItemDto>.Failure() : Result<CartItemDto>.Success(cartItemDto);
            });
            cartsRepo.Setup(r => r.GetUserCartItemsDtosAsync(It.IsAny<string>())).ReturnsAsync((string userId) =>
            {
                return Result<List<CartItemDto>>.Success(cartItemsDtos);
            });
            mockUsersService.Setup(r => r.GetUserId()).Returns("1");

            productsRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
            {
                return products.FirstOrDefault(p => p.Id == id);
            });

            cartsRepo.Setup(r => r.GetUserCartItemDtoByProductIdAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((string userId, int productId) =>
            {
                var cartItemDto = cartItemsDtos.FirstOrDefault(p => p.ProductId == productId);
                return (cartItemDto == null) ? Result<CartItemDto>.Failure() : Result<CartItemDto>.Success(cartItemDto);
            });

            mapperMock
            .Setup(m => m.Map<CartItemDto>(It.IsAny<CartItem>()))
            .Returns((CartItem ci) => cartItemsDtos.FirstOrDefault(p => p.Id == ci.Id));

            mapperMock
            .Setup(m => m.Map<CartItem>(It.IsAny<CartItemDto>()))
            .Returns((CartItemDto ci) => cartItems.FirstOrDefault(p => p.Id == ci.Id));

            mapperMock.Setup(m => m.Map<List<CartItem>>(cartItemsDtos))
                      .Returns(cartItems);

            mapperMock.Setup(m => m.Map<List<CartItemDto>>(cartItems))
            .Returns(cartItemsDtos);

            mapperMock.Setup(m => m.Map<List<CartItem>>(cartItemsDtos))
                      .Returns(cartItems);

            return (cartsRepo, productsRepo, mockUsersService, mapperMock);
        }
    }
}
