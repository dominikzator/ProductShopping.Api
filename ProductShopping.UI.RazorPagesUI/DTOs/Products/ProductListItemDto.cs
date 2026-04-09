namespace ProductShopping.UI.RazorPagesUI.DTOs.Products;

public sealed class ProductListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public string ImageUrl { get; set; }
}