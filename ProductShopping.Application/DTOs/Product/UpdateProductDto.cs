using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Application.DTOs.Product;

public class UpdateProductDto : CreateProductDto
{
    [Required]
    public int Id { get; set; }
}
