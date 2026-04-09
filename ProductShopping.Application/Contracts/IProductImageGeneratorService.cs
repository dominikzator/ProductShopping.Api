using ProductShopping.Application.DTOs;

namespace ProductShopping.Application.Contracts
{
    public interface IProductImageGeneratorService
    {
        Task<List<GeneratedImageDto>> GenerateImagesForFirstProductsAsync(int count);
        Task<GeneratedImageDto> GenerateProductImageAsync(int productId);
    }
}