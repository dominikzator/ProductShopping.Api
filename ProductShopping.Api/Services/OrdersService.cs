using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Order;
using ProductShopping.Api.DTOs.Product;
using ProductShopping.Api.Models;
using ProductShopping.Api.Results;
using Serilog;
using System.Security.Claims;

namespace ProductShopping.Api.Services;

public class OrdersService(ProductShoppingDbContext context, ICartItemsService cartItemsService, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IOrdersService
{
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

        var outputDto = mapper.Map<GetOrderDto>(createdOrder);

        return Result<GetOrderDto>.Success(outputDto);
    }

    private string GenerateOrderNumber()
    {
        return $"ORDER-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
