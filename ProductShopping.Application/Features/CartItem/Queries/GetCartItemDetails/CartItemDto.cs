namespace ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;

public class CartItemDto
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OverallPrice { get; set; }
    public double Rating { get; set; }
    public int Quantity { get; set; }
}
