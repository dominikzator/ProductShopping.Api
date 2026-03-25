using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HR.LeaveManagement.Application.Contracts.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using System.Text;
using System.Text.Json;

namespace ProductShopping.Infrastructure.Services;

public class ProductImageGeneratorService : IProductImageGeneratorService
{
    private readonly IProductsRepository _productsRepository;
    private readonly BlobContainerClient _blobClient;
    private readonly IConfiguration _config;
    private readonly IAppLogger<ProductImageGeneratorService> _logger;

    public ProductImageGeneratorService(
        IProductsRepository productsRepository,
        IConfiguration config,
        BlobServiceClient blobServiceClient,
        IAppLogger<ProductImageGeneratorService> logger)
    {
        _productsRepository = productsRepository;
        _config = config;
        _blobClient = blobServiceClient.GetBlobContainerClient("productshoppingapi");
        _logger = logger;
    }

    public async Task<GeneratedImageDto> GenerateProductImageAsync(int productId)
    {
        var product = await _productsRepository.GetByIdAsync(productId) ?? throw new Exception($"Product {productId} not found");

        var prompt = $"Product of category: {product.Category.Name}, Name: {product.Name}";

        _logger.LogInformation("Generating Image for Product {ProductId}: {Prompt}",
            productId, prompt);

        var imageBytes = await GenerateImageRawAsync(prompt);

        var productSuffix = product.Name.ToLower().Replace(' ', '_');

        var blobName = $"products/{productId}-{productSuffix}.jpg";
        var blobClient = _blobClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(new BinaryData(imageBytes),
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" } });

        var imageUrl = blobClient.Uri.ToString();

        _logger.LogInformation("Image saved: {ImageUrl}", imageUrl);

        return new GeneratedImageDto
        {
            ProductId = productId,
            ImageUrl = imageUrl,
            Prompt = prompt,
            BlobName = blobName,
            GeneratedAt = DateTime.UtcNow
        };
    }
    private async Task<byte[]> GenerateImageRawAsync(string prompt)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", _config["StabilityAI:ApiKey"]);

        var payload = new
        {
            text_prompts = new object[]
            {
            new { text = prompt },
            new { text = "blurry, deformed, low quality", weight = -1 }
        },
            cfg_scale = 5,
            height = 1024,
            width = 1024,
            samples = 1,
            steps = 15,
            sampler = "K_EULER"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/text-to-image", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Stability v1 API: {response.StatusCode} - {error}");
        }

        var result = JsonSerializer.Deserialize<StabilityV1Response>(await response.Content.ReadAsStringAsync());
        return Convert.FromBase64String(result.artifacts[0].base64);
    }
}

public class StabilityV1Response
{
    public ArtifactV1[] artifacts { get; set; }
}

public class ArtifactV1
{
    public string base64 { get; set; }
    public string finishReason { get; set; }
}