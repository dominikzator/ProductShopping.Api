using ProductShopping.Application.DTOs;

namespace ProductShopping.Application.Contracts
{
    public interface IProductImageGeneratorService
    {
        Task<GeneratedImageDto> GenerateProductImageAsync(int productId);
    }
}