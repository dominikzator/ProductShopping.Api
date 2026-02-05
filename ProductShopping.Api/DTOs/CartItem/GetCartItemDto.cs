namespace ProductShopping.Api.DTOs.CartItem;

public class GetCartItemDto
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public decimal OverallPrice { get; set; }
    public double Rating { get; set; }
    public int Quantity { get; set; }
}
