using ProductShopping.Api.DTOs.ImageGeneration;

namespace ProductShopping.Api.Contracts
{
    public interface IProductImageGeneratorService
    {
        Task<GeneratedImageDto> GenerateProductImageAsync(int productId);
    }
}