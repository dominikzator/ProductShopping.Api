using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShopping.Application.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public string UserId { get; set; }

    public List<CartItemDto> CartItems { get; set; }
}
