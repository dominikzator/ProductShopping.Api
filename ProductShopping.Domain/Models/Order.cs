using ProductShopping.Domain.Enums;

namespace ProductShopping.Domain.Models;

public class Order
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; }
    public string CustomerId { get; set; }
    public Address Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public OrderStatus OrderStatus { get; set; } = 0;

    public List<OrderItem> OrderItems{ get; set; }
}
