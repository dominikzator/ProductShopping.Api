using ProductShopping.Api.Constants;
using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Api.DTOs.Order;

public class GetOrderDto
{
    public string OrderId { get; set; }
    public string OrderNumber { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Street { get; set; }
    public string? BuildingNumber { get; set; }
    public string? ApartmentNumber { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string? PhoneNumber { get; set; }
    public string Status { get; set; }
}
