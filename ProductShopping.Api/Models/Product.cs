using ProductShopping.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace ProductShopping.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int CategoryId { get; set;}
    public ProductCategory Category { get; set;}

    public decimal Price { get; set; }
    public decimal Rating { get; set; }
}
