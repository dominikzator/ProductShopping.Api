using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IOrdersRepository : IGenericRepository<Order>
    {
        Task<Result<bool>> AddOrderItemAsync(OrderItem orderItem);
        Result<decimal> GetOrderItemsTotalPrice(List<OrderItemDto> orderItems);
        Task<Result<OrderDto>> GetUserOrderByOrderNumberAsync(string userId, string orderNumber);
        Task<Result<OrderDto>> GetUserOrderAsync(string userId, string orderId);
        Task<Result<OrderItemDto>> GetUserOrderItemAsync(string userId, string orderItemId);
        Task<Result<List<OrderItemDto>>> GetUserOrderItemsByOrderIdAsync(string userId, string orderId);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<Result<bool>> RemoveOrderItemAsync(OrderItem orderItem);
        Task<Result<bool>> RemoveOrderItemsAsync(List<OrderItem> orderItems);
        Task<Result<bool>> UpdateOrderItemAsync(OrderItem orderItem);
        Task<Result<bool>> SetUserOrderPayedAsync(string userId, int orderId);
    }
}