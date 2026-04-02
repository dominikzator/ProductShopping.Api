using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.Contracts.Persistence
{
    public interface IOrdersRepository : IGenericRepository<Order>
    {
        Task<Result<bool>> AddOrderItemAsync(OrderItem orderItem);
        Result<decimal> GetOrderItemsTotalPrice(List<OrderItemDto> orderItems);
        Task<Result<OrderDto>> GetUserOrderDtoByOrderNumberAsync(string userId, string orderNumber);
        Task<Result<OrderDto>> GetUserOrderDtoAsync(string userId, int orderId);
        Task<Result<OrderItemDto>> GetUserOrderItemDtoAsync(string userId, int orderItemId);
        Task<Result<List<OrderItemDto>>> GetUserOrderItemsDtosByOrderIdAsync(string userId, int orderId);
        Task<List<OrderDto>> GetUserOrdersDtosAsync(string userId);
        Task<Result<bool>> RemoveOrderItemAsync(OrderItem orderItem);
        Task<Result<bool>> RemoveOrderItemsAsync(List<OrderItem> orderItems);
        Task<Result<bool>> UpdateOrderItemAsync(OrderItem orderItem);
        Task<Result<bool>> SetUserOrderPayedAsync(string userId, int orderId);
        Task<List<Order>> GetUserOrdersAsync(string userId, bool tracking = false);
        Task<Order> GetUserOrderAsync(string userId, int orderId, bool tracking = false);
    }
}