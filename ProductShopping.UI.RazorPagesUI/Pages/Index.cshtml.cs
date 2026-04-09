using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [BindProperty(SupportsGet = true)]
        public ProductsQueryDto Query { get; set; } = new();

        public Dictionary<string, string?> CurrentRouteValues { get; private set; } = new();

        public List<string> CategoryOptions { get; private set; }

        [TempData]
        public string? FlashMessage { get; set; }

        [TempData]
        public string? FlashMessageType { get; set; }

        public async Task OnGetAsync(CancellationToken ct)
        {
            Console.WriteLine("Query.CategoryName" + Query.CategoryName);

            Query.PageNumber = Query.PageNumber <= 0 ? 1 : Query.PageNumber;
            Query.PageSize = Query.PageSize <= 0 ? 10 : Query.PageSize;

            Products = await _productsApiClient.GetProductsAsync(Query, ct);

            CategoryOptions = await _productsApiClient.GetCategoryNamesAsync();

            CurrentRouteValues = Request.Query
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);

            CurrentRouteValues["Query.SortDescending"] = Query.SortDescending.ToString().ToLowerInvariant();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId, int quantity, CancellationToken ct)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return new JsonResult(new
                {
                    success = false,
                    type = "error",
                    message = "Nie jesteœ zalogowany. Zaloguj siê, aby dodaæ produkt do koszyka."
                });
            }

            //await _productsApiClient.addAddToCartAsync(productId, quantity, ct);

            return new JsonResult(new
            {
                success = true,
                type = "success",
                message = "Produkt zosta³ dodany do koszyka."
            });
        }
    }
}
