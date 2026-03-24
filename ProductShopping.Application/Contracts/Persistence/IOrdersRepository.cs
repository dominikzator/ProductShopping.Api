using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IOrdersRepository : IGenericRepository<Order>
    {
        Task<Result<bool>> AddOrderItemAsync(OrderItem orderItem);
        Result<decimal> GetOrderItemsTotalPrice(List<OrderItem> orderItems);
        Task<Result<Order>> GetUserOrderByOrderNumberAsync(string userId, string orderNumber);
        Task<Result<Order>> GetUserOrderAsync(string userId, string orderId);
        Task<Result<OrderItem>> GetUserOrderItemAsync(string userId, string orderItemId);
        Task<Result<List<OrderItem>>> GetUserOrderItemsByOrderIdAsync(string userId, string orderId);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Result<bool>> RemoveOrderItemAsync(OrderItem orderItem);
        Task<Result<bool>> RemoveOrderItemsAsync(List<OrderItem> orderItems);
    }
}