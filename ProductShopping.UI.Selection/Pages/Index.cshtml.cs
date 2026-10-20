using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ProductShopping.UI.Selection.Pages;

public class IndexModel(IOptions<FrontendLinksOptions> options) : PageModel
{
    public FrontendLinksOptions Links { get; } = options.Value;

    public void OnGet()
    {
    }
}
