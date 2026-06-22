using ProductShopping.Application.Features.Order.Commands.CreateOrder;
using ProductShopping.Application.Features.Order.Commands.UpdateOrder;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;

namespace ProductShopping.UI.Shared.Contracts
{
    public interface IOrdersApiClient
    {
        Task<OrderDto?> CreateOrderAsync(string accessToken, CreateOrderCommand command, CancellationToken ct);
        Task DeleteOrderAsync(string accessToken, int orderId, CancellationToken ct);
        Task<OrderDto?> GetOrderAsync(string accessToken, int orderId, CancellationToken ct);
        Task<PagedResult<OrderDto>> GetOrdersAsync(string accessToken, PaginationParameters paginationParameters, CancellationToken ct);
        Task UpdateOrderAsync(string accessToken, int id, UpdateOrderCommand command, CancellationToken ct);
    }
}