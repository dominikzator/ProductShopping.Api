using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Api.DTOs.Product;

public class UpdateProductDto : CreateProductDto
{
    [Required]
    public int Id { get; set; }
}
