using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Logging;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Enums;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Persistence.Repositories;

public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
{
    private readonly ProductShoppingDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAppLogger<OrdersRepository> _logger;

    public OrdersRepository(ProductShoppingDbContext context, IMapper mapper, IAppLogger<OrdersRepository> logger) : base(context)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _context.Orders.Where(o => o.CustomerId == userId).Include(o => o.OrderItems).AsNoTracking().ToListAsync();
        var ordersDtos = _mapper.Map<List<OrderDto>>(orders);

        return ordersDtos;
    }

    public async Task<Result<OrderDto>> GetUserOrderAsync(string userId, string orderId)
    {
        var userOrders = await GetUserOrdersAsync(userId);
        var userOrder = userOrders.FirstOrDefault(o => o.Id.ToString() == orderId);

        if (userOrder == null)
        {
            return Result<OrderDto>.Failure(new Error(ErrorCodes.NotFound, $"User Order of ID '{orderId}' does not exist."));
        }

        return Result<OrderDto>.Success(userOrder);
    }

    public async Task<Result<OrderDto>> GetUserOrderByOrderNumberAsync(string userId, string orderNumber)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.CustomerId == userId && o.OrderNumber == orderNumber);

        if (order == null)
        {
            return Result<OrderDto>.Failure(new Error(ErrorCodes.NotFound, $"User Order of Order Number: '{orderNumber}' does not exist."));
        }
        var orderDto = _mapper.Map<OrderDto>(order);

        return Result<OrderDto>.Success(orderDto);
    }

    public async Task<Result<OrderItemDto>> GetUserOrderItemAsync(string userId, string orderItemId)
    {
        var orderItem = await _context.OrderItems.Include(o => o.Order).Include(o => o.Product).FirstOrDefaultAsync(o => o.CustomerId == userId && o.Id.ToString() == orderItemId);

        if (orderItem == null)
        {
            return Result<OrderItemDto>.Failure(new Error(ErrorCodes.NotFound, $"User Order Item of ID '{orderItemId}' does not exist."));
        }
        var orderItemDto = _mapper.Map<OrderItemDto>(orderItem);

        return Result<OrderItemDto>.Success(orderItemDto);
    }

    public async Task<Result<List<OrderItemDto>>> GetUserOrderItemsByOrderIdAsync(string userId, string orderId)
    {
        var orderItems = await _context.OrderItems.Include(o => o.Order).Include(o => o.Product).Where(o => o.CustomerId == userId && o.OrderId.ToString() == orderId).ToListAsync();
        var orderItemsDtos = _mapper.Map<List<OrderItemDto>>(orderItems);

        return Result<List<OrderItemDto>>.Success(orderItemsDtos);
    }

    public async Task<Result<bool>> AddOrderItemAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveOrderItemsAsync(List<OrderItem> orderItems)
    {
        _context.OrderItems.RemoveRange(orderItems);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public Result<decimal> GetOrderItemsTotalPrice(List<OrderItemDto> orderItems)
    {
        decimal totalPrice = orderItems.Sum(o => o.TotalPrice);

        return Result<decimal>.Success(totalPrice);
    }

    public async Task<Result<bool>> SetUserOrderPayedAsync(string userId, int orderId)
    {
        _logger.LogInformation($"SetUserOrderPayedAsync: userId: {userId}, orderId: {orderId}");

        var orderResult = await GetUserOrderAsync(userId, orderId.ToString());

        _logger.LogInformation($"SetUserOrderPayedAsync after taking orderResult");

        var orderDto = orderResult.Value;

        var order = await _context.Orders
        .Include(o => o.OrderItems)
        .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId);

        if(order == null)
        {
            _logger.LogInformation("Order NULL!");
        }

        order!.OrderStatus = OrderStatus.Payed;
        await UpdateAsync(order);

        foreach (var orderItem in order.OrderItems)
        {
            orderItem.OrderStatus = OrderStatus.Payed;
            await UpdateOrderItemAsync(orderItem);
        }

        return Result<bool>.Success(true);
    }
}
