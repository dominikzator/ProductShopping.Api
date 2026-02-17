namespace ProductShopping.Api.DTOs.ImageGeneration;

public class GeneratedImageDto
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }
    public string Prompt { get; set; }
    public string BlobName { get; set; }
    public DateTime GeneratedAt { get; set; }
}
