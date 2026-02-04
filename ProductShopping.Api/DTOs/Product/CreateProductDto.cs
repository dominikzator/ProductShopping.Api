using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Api.DTOs.Product;

public class CreateProductDto
{
    [Required]
    public required string Name { get; set; }

    [Range(1.0, 5.0)]
    public double Rating { get; set; }

    [Range(0.01, 5000)]
    public decimal Price { get; set; }

    public string CategoryName { get; set; }
}
