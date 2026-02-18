using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.DTOs.Order;
using ProductShopping.Api.DTOs.Product;
using ProductShopping.Api.Models;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Results;
using Serilog;
using System.Security.Claims;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ProductShopping.Api.Services;

public class OrdersService(ProductShoppingDbContext context, ILogger<OrdersService> logger, IConfiguration config, ICartItemsService cartItemsService, IPaymentsService paymentsService, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IOrdersService
{
    public async Task<Result<PagedResult<GetOrderDto>>> GetOrdersAsync(PaginationParameters paginationParameters)
    {
        var query = context.Orders.AsQueryable();

        var userId = httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userOrders = await query.Where(o => o.CustomerId == userId).Include(o => o.OrderItems).ToListAsync();

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
        var userId = httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var order = await context.Orders.Where(o => o.CustomerId == userId).Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);

        if (order is null)
        {
            return Result<GetOrderDto>.Failure(new Error(ErrorCodes.NotFound, $"Order '{id}' was not found."));
        }

        var outputDto = mapper.Map<GetOrderDto>(order);

        return Result<GetOrderDto>.Success(outputDto);
    }

    public async Task<Result<PagedResult<GetOrderItemDto>>> GetOrderItemsAsync(int orderID, PaginationParameters paginationParameters)
    {
        var userId = httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var order = await context.Orders.Where(o => o.CustomerId == userId).Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderID);

        if (order is null)
        {
            return Result<PagedResult<GetOrderItemDto>>.Failure(new Error(ErrorCodes.NotFound, $"Order '{orderID}' was not found."));
        }

        var userOrderItems = order.OrderItems.ToList();

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
        var orderItem = await context.OrderItems.FirstOrDefaultAsync(item => item.OrderItemId == orderItemID);

        if (orderItem is null)
        {
            return Result<GetOrderItemDto>.Failure(new Error(ErrorCodes.NotFound, $"Order Item '{orderItemID}' was not found."));
        }

        var outputDto = mapper.Map<GetOrderItemDto>(orderItem);

        return Result<GetOrderItemDto>.Success(outputDto);
    }

    public async Task<Result<GetOrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        var userId = httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userCart = context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefault(cart => cart.UserId == userId);

        if (userCart == null)
        {
            return Result<GetOrderDto>.Failure(new Error(ErrorCodes.Failure,
                $"User '{httpContextAccessor!.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value}' " +
                $"does not have a Cart. This should not happen. Contact developers."));
        }

        if (userCart.CartItems.Count == 0)
        {
            return Result<GetOrderDto>.Failure(new Error(ErrorCodes.Failure, $"User Cart is empty"));
        }
        Log.Information($"userCart.CartItems.Count: {userCart.CartItems.Count}");

        var orderNumber = GenerateOrderNumber();

        var order = new Order
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Address = createOrderDto.Address,
            CustomerId = userId,
            OrderNumber = orderNumber
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var createdOrder = await context.Orders.FirstAsync(o => o.OrderNumber == orderNumber);

        foreach (var item in userCart.CartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = createdOrder.OrderId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CustomerId = userId,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price,
                TotalPrice = item.Product.Price * item.Quantity
            };
            context.OrderItems.Add(orderItem);
            await context.SaveChangesAsync();
        }
        await cartItemsService.ClearCartAsync();

        var orderItems = await context.OrderItems.Where(o => o.OrderId == createdOrder.OrderId).ToListAsync();

        string domainName = config["Constants:DomainName"];

        logger.LogInformation($"domainName: {domainName}");

        var session = await paymentsService.CreatePaymentSessionAsync(new DTOs.Payment.PaymentRequestDto
        {
            OrderId = createdOrder.OrderId,
            Domain = domainName,
            OrderNumber = createdOrder.OrderNumber,
            Items = mapper.Map<List<GetOrderItemDto>>(orderItems),
            TotalPrice = orderItems.Sum(o => o.TotalPrice)
        });

        var outputDto = mapper.Map<GetOrderDto>(createdOrder);
        outputDto.PaymentUrl = session.Url;

        Console.WriteLine($"Order ID: {session.ClientReferenceId}");

        return Result<GetOrderDto>.Success(outputDto);
    }

    private string GenerateOrderNumber()
    {
        return $"ORDER-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public async Task<Result> DeleteOrderAsync(int orderId)
    {
        var order = await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order '{orderId}' was not found."));
        }

        context.OrderItems.RemoveRange(order.OrderItems);
        context.Orders.Remove(order);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteOrderItemAsync(int orderItemId)
    {
        var orderItem = await context.OrderItems.FirstOrDefaultAsync(item => item.OrderItemId == orderItemId);

        if (orderItem is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Order Item '{orderItemId}' was not found."));
        }

        context.OrderItems.Remove(orderItem);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}
