using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Api.DTOs.CartItem;

public class RemoveCartItemDto
{
    [Required]
    public int CartItemId { get; set; }

    [Required, Range(1, 1000)]
    public int Quantity { get; set; }
}
