using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.UI.RazorPagesUI.Clients;
using ProductShopping.UI.RazorPagesUI.Contracts;
using ProductShopping.UI.RazorPagesUI.DTOs.Products;

namespace ProductShopping.UI.RazorPagesUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductsApiClient _productsApiClient;

        public IndexModel(IProductsApiClient productsApiClient)
        {
            _productsApiClient = productsApiClient;
        }

        public PagedResultDto<ProductListItemDto> Products { get; private set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Products = await _productsApiClient.GetProductsAsync(cancellationToken);
            Console.WriteLine("Products.Count: " + Products.Data.Count());
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity, int pageNumber = 1)
        {
            Console.WriteLine($"productId: {productId}, quantity: {quantity}, pageNumber: {pageNumber}");

            if (quantity < 1) quantity = 1;
            if (quantity > 999) quantity = 999;

            // add to cart

            return RedirectToPage("./Index", new { pageNumber });
        }
    }
}
