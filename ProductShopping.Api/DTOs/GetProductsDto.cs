namespace ProductShopping.Api.DTOs;

public class GetProductsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
}
