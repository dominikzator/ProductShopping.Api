using ProductShopping.Application.DTOs.Order;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;

namespace ProductShopping.Application.Contracts
{
    public interface IOrdersService
    {
        Task<Result<PagedResult<GetOrderDto>>> GetOrdersAsync(PaginationParameters paginationParameters);
        Task<Result<GetOrderDto>> CreateOrder(CreateOrderDto createOrderDto);
        Task<Result<GetOrderDto>> GetOrderAsync(int id);
        Task<Result<PagedResult<GetOrderItemDto>>> GetOrderItemsAsync(int orderID, PaginationParameters paginationParameters);
        Task<Result<GetOrderItemDto>> GetOrderItemAsync(int orderItemID);
        Task<Result> DeleteOrderAsync(int orderId);
        Task<Result> DeleteOrderItemAsync(int orderItemId);
    }
}