using AutoMapper;
using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.DTOs.Product;
using ProductShopping.Api.Models;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, GetProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<Product, GetProductDto>();

        CreateMap<CreateCartItemDto, CartItem>();
        CreateMap<CartItem, GetCartItemDto>().AfterMap((src, dest) =>
        {
            dest.CategoryName = src.Product.Category.Name;
            dest.Name = src.Product.Name;
            dest.Rating = src.Product.Rating;
            dest.OverallPrice = dest.Quantity * src.Product.Price;
        });
    }
}