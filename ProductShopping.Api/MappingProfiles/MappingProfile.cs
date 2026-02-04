using AutoMapper;
using ProductShopping.Api.DTOs;
using ProductShopping.Api.Models;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, GetProductsDto>();
    }
}