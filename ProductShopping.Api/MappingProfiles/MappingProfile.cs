using AutoMapper;
using ProductShopping.Api.DTOs.CartItem;
using ProductShopping.Api.DTOs.Order;
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

        CreateMap<Order, GetOrderDto>().AfterMap((src, dest) =>
        {
            dest.OwnerId = src.CustomerId;
            dest.Street = src.Address.Street;
            dest.BuildingNumber = src.Address.BuildingNumber;
            dest.ApartmentNumber = src.Address.ApartmentNumber;
            dest.City = src.Address.City;
            dest.PostalCode = src.Address.PostalCode;
            dest.Country = src.Address.Country;
            dest.PhoneNumber = src.Address.PhoneNumber;
            dest.Status = src.OrderStatus.ToString();
        });

        CreateMap<OrderItem, GetOrderItemDto>().AfterMap((src, dest) =>
        {

        });
    }
}