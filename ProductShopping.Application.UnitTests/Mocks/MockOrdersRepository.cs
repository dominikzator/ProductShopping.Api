using AutoMapper;
using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.UnitTests.Mocks
{
    public class MockOrdersRepository
    {
        public static (Mock<IOrdersRepository>, Mock<IUsersService>, Mock<IMapper>, Mock<ICartsRepository>) GetOrdersRepository_OrdersSetup()
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 3333,
                    Quantity = 3
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new OrderItem
                {
                    Id = 3,
                    OrderId = 1,
                    ProductId = 3335,
                    Quantity = 5
                }
            };

            var orderItemsDtos = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 3333,
                    Quantity = 3
                },
                new OrderItemDto
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 3334,
                    Quantity = 4
                },
                new OrderItemDto
                {
                    Id = 3,
                    OrderId = 1,
                    ProductId = 3335,
                    Quantity = 5
                }
            };

            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    CustomerId = "1",
                    OrderNumber = "1",
                    OrderItems = orderItems,
                },
                new Order
                {
                    Id = 2,
                    CustomerId = "1",
                },
                new Order
                {
                    Id = 3,
                    CustomerId = "1",
                },
            };

            var ordersDtos = new List<OrderDto>
            {
                 new OrderDto
                {
                    Id = 1,
                    OwnerId = "1",
                    OrderNumber = "1",
                    OrderItems = orderItemsDtos,
                },
                new OrderDto
                {
                    Id = 2,
                    OwnerId = "1",
                },
                new OrderDto
                {
                    Id = 3,
                    OwnerId = "1",
                },
            };

            var cart = new Cart
            {
                Id = 1,
                UserId = "1",
                CartItems = new []{ 
                    new CartItem
                    {
                        Id = 1,
                        CartId = 1,
                        ProductId = 3333,
                        Quantity = 3
                    } 
                }.ToList(),
            };

            var cartDto = new CartDto
            {
                Id = 1,
                UserId = "1",
                CartItems = new[] {
                    new CartItemDto
                    {
                        Id = 1,
                        CartId = 1,
                        ProductId = 3333,
                        Quantity = 3
                    }
                }.ToList()
            };

            var mockRepo = new Mock<IOrdersRepository>();
            var mockUsersService = new Mock<IUsersService>();
            var mapperMock = new Mock<IMapper>();
            var cartsMock = new Mock<ICartsRepository>();

            cartsMock.Setup(r => r.GetUserCartDtoAsync(It.IsAny<string>())).ReturnsAsync((string userId) =>
            {
                return (userId == cart.UserId) ? Result<CartDto>.Success(cartDto) : Result<CartDto>.Failure();
            });

            mockUsersService.Setup(r => r.GetUserId()).Returns("1");

            mockRepo.Setup(r => r.GetUserOrderDtoAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((string userId, int orderId) =>
            {
                var orderDto = ordersDtos.FirstOrDefault(p => p.Id == orderId);
                return (orderDto != null) ? Result<OrderDto>.Success(orderDto) : Result<OrderDto>.Failure();
            });
            mockRepo.Setup(r => r.GetUserOrderDtoByOrderNumberAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string userId, string orderNumber) =>
            {
                return Result<OrderDto>.Success(ordersDtos[0]);
            });
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int orderId) =>
            {
                var order = orders.FirstOrDefault(orders => orders.Id == orderId);
                return order;
            });

            //mapperMock.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);
            //mapperMock.Setup(m => m.Map<Order>(orderDto)).Returns(order);

            mapperMock.Setup(m => m.Map<List<OrderDto>>(orders)).Returns(ordersDtos);
            mapperMock.Setup(m => m.Map<List<Order>>(ordersDtos)).Returns(orders);

            mapperMock.Setup(m => m.Map<List<OrderItemDto>>(orderItems)).Returns(orderItemsDtos);
            mapperMock.Setup(m => m.Map<List<OrderItem>>(orderItemsDtos)).Returns(orderItems);

            mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns((Order order) =>
            {
                return new OrderDto
                {
                    Id = order.Id,
                    OrderItems = mapperMock.Object.Map<List<OrderItemDto>>(order.OrderItems),
                };
            });
            mapperMock.Setup(m => m.Map<Order>(It.IsAny<OrderDto>())).Returns((OrderDto orderDto) =>
            {
                return new Order
                {
                    Id = orderDto.Id,
                    OrderItems = mapperMock.Object.Map<List<OrderItem>>(orderDto.OrderItems),
                };
            });

            mockRepo.Setup(r => r.GetUserOrdersDtosAsync(It.IsAny<string>())).ReturnsAsync((string userId) =>
            {
                return ordersDtos;
            });

            return (mockRepo, mockUsersService, mapperMock, cartsMock);
        }
    }
}
