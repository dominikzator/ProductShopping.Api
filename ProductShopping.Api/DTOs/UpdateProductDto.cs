using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Api.DTOs;

public class UpdateProductDto : CreateProductDto
{
    [Required]
    public int Id { get; set; }
}
