using ProductShopping.Domain.Models;

namespace ProductShopping.Api.DTOs.Order;

public class CreateOrderDto
{
    public Address Address { get; set; }
}
