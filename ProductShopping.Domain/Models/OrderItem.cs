using ProductShopping.Domain.Common;
using ProductShopping.Domain.Enums;

namespace ProductShopping.Domain.Models;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order {  get; set; }

    public string CustomerId { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public string? ProductName { get; set; }

    public OrderStatus OrderStatus { get; set; } = 0;
}
