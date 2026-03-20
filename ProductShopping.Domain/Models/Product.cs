namespace ProductShopping.Domain.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int CategoryId { get; set;}
    public ProductCategory Category { get; set;}

    public decimal Price { get; set; }
    public double Rating { get; set; }
}
