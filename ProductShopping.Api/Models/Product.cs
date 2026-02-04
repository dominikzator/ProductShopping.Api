namespace ProductShopping.Api.Models;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }

    public int ProductCategoryId { get; set;}
    public ProductCategory ProductCategory { get; set;}

    public decimal ProductPrice { get; set; }
    public decimal ProductRating { get; set; }
}
