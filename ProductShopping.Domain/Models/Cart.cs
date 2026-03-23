using ProductShopping.Domain.Common;

namespace ProductShopping.Domain.Models;

public class Cart : BaseEntity
{
    public string UserId { get; set; }
    //public ApplicationUser User { get; set; }

    public List<CartItem> CartItems { get; set; }
}
