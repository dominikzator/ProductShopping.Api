using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Api.DTOs.CartItem;

public class CreateCartItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required, Range(1,1000)]
    public int Quantity { get; set; }
}
