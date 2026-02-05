namespace ProductShopping.Api.Models;

public class Cart
{
    public int CartId { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
