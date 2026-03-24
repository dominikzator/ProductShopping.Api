using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Application.Constants;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs.Product;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;

namespace ProductShopping.Persistence.Repositories;

public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
{
    private readonly ProductShoppingDbContext _context;
    private readonly IMapper _mapper;

    public OrdersRepository(ProductShoppingDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Order>> GetUserOrdersAsync(string userId) => await _context.Orders.Where(o => o.CustomerId == userId).Include(o => o.OrderItems).ToListAsync();

    public async Task<Result<Order>> GetUserOrderAsync(string userId, string orderId)
    {
        var userOrders = await GetUserOrdersAsync(userId);

        var userOrder = userOrders.FirstOrDefault(o => o.Id.ToString() == orderId);

        if (userOrder == null)
        {
            return Result<Order>.Failure(new Error(ErrorCodes.NotFound, $"User Order of ID '{orderId}' does not exist."));
        }

        return Result<Order>.Success(userOrder);
    }

    public async Task<Result<Order>> GetUserOrderByOrderNumberAsync(string userId, string orderNumber)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.CustomerId == userId && o.OrderNumber == orderNumber);

        if (order == null)
        {
            return Result<Order>.Failure(new Error(ErrorCodes.NotFound, $"User Order of Order Number: '{orderNumber}' does not exist."));
        }

        return Result<Order>.Success(order);
    }

    public async Task<Result<OrderItem>> GetUserOrderItemAsync(string userId, string orderItemId)
    {
        var orderItem = await _context.OrderItems.Include(o => o.Order).Include(o => o.Product).FirstOrDefaultAsync(o => o.CustomerId == userId && o.Id.ToString() == orderItemId);

        if (orderItem == null)
        {
            return Result<OrderItem>.Failure(new Error(ErrorCodes.NotFound, $"User Order Item of ID '{orderItemId}' does not exist."));
        }

        return Result<OrderItem>.Success(orderItem);
    }

    public async Task<Result<List<OrderItem>>> GetUserOrderItemsByOrderIdAsync(string userId, string orderId)
    {
        var orderItems = await _context.OrderItems.Include(o => o.Order).Include(o => o.Product).Where(o => o.CustomerId == userId && o.OrderId.ToString() == orderId).ToListAsync();

        return Result<List<OrderItem>>.Success(orderItems);
    }

    public async Task<Result<bool>> AddOrderItemAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
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

    public Result<decimal> GetOrderItemsTotalPrice(List<OrderItem> orderItems)
    {
        decimal totalPrice = orderItems.Sum(o => o.TotalPrice);

        return Result<decimal>.Success(totalPrice);
    }
}
