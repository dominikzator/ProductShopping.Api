using ProductShopping.Api.DTOs.Order;

namespace ProductShopping.Api.DTOs.Payment;

public class PaymentRequestDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; }
    public string Domain { get; set; }
    public List<GetOrderItemDto> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public string UserEmail { get; set; }
}