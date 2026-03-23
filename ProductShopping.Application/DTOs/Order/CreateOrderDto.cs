using ProductShopping.Domain.Models;

namespace ProductShopping.Application.DTOs.Order;

public class CreateOrderDto
{
    public Address Address { get; set; }
}
