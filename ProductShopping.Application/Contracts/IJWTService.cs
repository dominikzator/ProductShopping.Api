using ProductShopping.Application.DTOs;

namespace ProductShopping.Application.Contracts;

public interface IJWTService
{
    Task<string> GenerateToken(UserDto user);
}