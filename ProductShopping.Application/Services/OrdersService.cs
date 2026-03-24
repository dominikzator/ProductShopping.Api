using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Order;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using System.Security.Claims;

namespace ProductShopping.Application.Services;

public class OrdersService(IOrdersRepository ordersRepository, ICartsRepository cartsRepository, IUsersService usersService, ILogger<OrdersService> logger,
    IConfiguration config, ICartItemsService cartItemsService, IPaymentsService paymentsService, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IOrdersService
{
    public async Task<Result<PagedResult<GetOrderDto>>> GetOrdersAsync(PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId();

        var userOrders = await ordersRepository.GetUserOrdersAsync(userId);

        var dtos = mapper.Map<List<GetOrderDto>>(userOrders);

        var pagedResult = new PagedResult<GetOrderDto>
        {
            Data = PagedResult<GetOrderDto>.GetData(dtos, paginationParameters),
            Metadata = PagedResult<GetOrderDto>.GetPaginationMetadata(dtos, paginationParameters)
        };

        return Result<PagedResult<GetOrderDto>>.Success(pagedResult);
    }

    public async Task<Result<GetOrderDto>> GetOrderAsync(int id)
    {
        var userId = usersService.GetUserId();

        var order = await ordersRepository.GetUserOrderAsync(userId, id.ToString());

        if (order.Value is null)
        {
            return Result<GetOrderDto>.Failure(new Error(ErrorCodes.NotFound, $"Order '{id}' was not found."));
        }

        var outputDto = mapper.Map<GetOrderDto>(order.Value);

        return Result<GetOrderDto>.Success(outputDto);
    }

    public async Task<Result<PagedResult<GetOrderItemDto>>> GetOrderItemsAsync(int orderID, PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId();

        var order = await ordersRepository.GetUserOrderAsync(userId, orderID.ToString());

        if (order.Value is null)
        {
            return Result<PagedResult<GetOrderItemDto>>.Failure(new Error(ErrorCodes.NotFound, $"Order '{orderID}' was not found."));
        }

        var userOrderItems = order.Value.OrderItems.ToList();

        var dtos = mapper.Map<List<GetOrderItemDto>>(userOrderItems);

        var pagedResult = new PagedResult<GetOrderItemDto>
        {
            Data = PagedResult<GetOrderItemDto>.GetData(dtos, paginationParameters),
            Metadata = PagedResult<GetOrderItemDto>.GetPaginationMetadata(dtos, paginationParameters)
        };

        return Result<PagedResult<GetOrderItemDto>>.Success(pagedResult);
    }

    public async Task<Result<GetOrderItemDto>> GetOrderItemAsync(int orderItemID)
    {
        var userId = usersService.GetUserId();

        var orderItem = ordersRepository.GetUserOrderItemAsync(userId, orderItemID.ToString());

        if (orderItem is null)
        {
            return Result<GetOrderItemDto>.Failure(new Error(ErrorCodes.NotFound, $"Order Item '{orderItemID}' was not found."));
        }

        var outputDto = mapper.Map<GetOrderItemDto>(orderItem);

        return Result<GetOrderItemDto>.Success(outputDto);
    }

    public async Task<Result<GetOrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        var userId = usersService.GetUserId();

        var userCart = await cartsRepository.GetUserCartAsync(userId);

        if(userCart.Value is null)
        {
            return Result<GetOrderDto>.Failure(new Error(ErrorCodes.Failure,
                $"User '{httpContextAccessor!.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        if (userCart.Value.CartItems.Count == 0)
        {
            return Result<GetOrderDto>.Failure(new Error(ErrorCodes.Failure, $"User Cart is empty"));
        }

        var orderNumber = GenerateOrderNumber();

        var order = new Order
        {
            Address = createOrderDto.Address,
            CustomerId = userId,
            OrderNumber = orderNumber
        };

        await ordersRepository.CreateAsync(order);

        var createdOrder = await ordersRepository.GetUserOrderByOrderNumberAsync(userId, orderNumber);

        foreach (var item in userCart.Value.CartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = createdOrder.Value!.Id,
                CustomerId = userId,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price,
                TotalPrice = item.Product.Price * item.Quantity
            };

            await ordersRepository.AddOrderItemAsync(orderItem);
        }
        await cartItemsService.ClearCartAsync();

        var orderItems = await ordersRepository.GetUserOrderItemsByOrderIdAsync(userId, createdOrder.Value!.Id.ToString());

        string domainName = config["Constants:DomainName"]!;

        var userEmail = usersService.GetUserEmail();

        var outputDto = await paymentsService.CreatePaymentSessionAsync(new DTOs.Payment.PaymentRequestDto
        {
            OrderId = createdOrder.Value.Id,
            Domain = domainName,
            OrderNumber = createdOrder.Value.OrderNumber,
            Items = mapper.Map<List<GetOrderItemDto>>(orderItems),
            TotalPrice = ordersRepository.GetOrderItemsTotalPrice(orderItems.Value!).Value,
            UserEmail = userEmail
        });

        return Result<GetOrderDto>.Success(outputDto);
    }

    private string GenerateOrderNumber()
    {
        return $"ORDER-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public async Task<Result> DeleteOrderAsync(int orderId)
    {
        var userId = usersService.GetUserId();

        var order = await ordersRepository.GetUserOrderAsync(userId, orderId.ToString());

        if (order.Value is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order '{orderId}' was not found."));
        }

        await ordersRepository.RemoveOrderItemsAsync(order.Value.OrderItems);
        await ordersRepository.DeleteAsync(order.Value);

        return Result.Success();
    }

    public async Task<Result> DeleteOrderItemAsync(int orderItemId)
    {
        var userId = usersService.GetUserId();

        var orderItem = await ordersRepository.GetUserOrderItemAsync(userId, orderItemId.ToString());

        if (orderItem.Value is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order Item '{orderItemId}' was not found."));
        }

        await ordersRepository.RemoveOrderItemAsync(orderItem.Value);

        return Result.Success();
    }
}