using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;

namespace ProductShopping.Application.DTOs.Payment;

public class PaymentRequestDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; }
    public string Domain { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public string UserEmail { get; set; }
}