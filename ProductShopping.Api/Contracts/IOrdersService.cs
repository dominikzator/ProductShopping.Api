using ProductShopping.Api.DTOs.Order;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IOrdersService
    {
        Task<Result<GetOrderDto>> CreateOrder(CreateOrderDto createOrderDto);
    }
}