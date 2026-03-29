using ProductShopping.Domain.Enums;

namespace ProductShopping.Application.Features.Order.Queries.GetOrderDetails;

public class OrderItemDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public string? ProductName { get; set; }

    public OrderStatus OrderStatus { get; set; } = 0;
}
