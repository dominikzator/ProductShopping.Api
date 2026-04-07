using ProductShopping.Domain.Enums;

namespace ProductShopping.Application.Features.Order.Queries.GetOrderDetails;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public string OwnerId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Street { get; set; }
    public string? BuildingNumber { get; set; }
    public string? ApartmentNumber { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string? PhoneNumber { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentUrl { get; set; }
}
