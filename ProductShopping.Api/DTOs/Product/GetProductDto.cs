namespace ProductShopping.Api.DTOs.Product;

public class GetProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
}
