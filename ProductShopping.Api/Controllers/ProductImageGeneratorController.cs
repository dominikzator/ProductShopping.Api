using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.DTOs;
using ProductShopping.Identity.Constants;

namespace ProductShopping.Api.Controllers;

// Controllers/ProductImagesController.cs
[ApiController]
[Route("api/products/{productId}/[controller]")]
[Authorize(Roles = RoleNames.Administrator)]  // Tylko admin
public class ProductImageGeneratorController(IProductImageGeneratorService productImageGeneratorService) : ControllerBase
{
    /// <summary>
    /// Generates AI Image of a Product with a given ID and saves it to Azure Blob Storage. Can be called only by an Administrator.
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<GeneratedImageDto>> GenerateImage(int productId)
    {
        try
        {
            var result = await productImageGeneratorService.GenerateProductImageAsync(productId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Błąd generowania obrazu", details = ex.Message });
        }
    }
}
