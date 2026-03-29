using AutoMapper;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.CartItem.Commands.AddCartItem;
using ProductShopping.Application.Features.CartItem.Queries.GetCartItemDetails;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Features.Product.Commands.CreateProduct;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.MappingProfiles;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductCommand, Product>();
        CreateMap<Cart, CartDto>();

        CreateMap<AddCartItemCommand, CartItem>();
        CreateMap<CartItem, CartItemDto>().AfterMap((src, dest) =>
        {
            dest.CategoryName = src.Product.Category.Name;
            dest.Name = src.Product.Name;
            dest.Rating = src.Product.Rating;
            dest.UnitPrice = src.Product.Price;
            dest.OverallPrice = dest.Quantity * src.Product.Price;
        }).ReverseMap();

        CreateMap<Order, OrderDto>().AfterMap((src, dest) =>
        {
            dest.OwnerId = src.CustomerId;
            dest.Street = src.Address.Street;
            dest.BuildingNumber = src.Address.BuildingNumber;
            dest.ApartmentNumber = src.Address.ApartmentNumber;
            dest.City = src.Address.City;
            dest.PostalCode = src.Address.PostalCode;
            dest.Country = src.Address.Country;
            dest.PhoneNumber = src.Address.PhoneNumber;
            dest.OrderStatus = src.OrderStatus;
        }).ReverseMap();

        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
    }
}