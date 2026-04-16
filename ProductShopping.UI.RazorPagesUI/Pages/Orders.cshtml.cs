using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.UI.RazorPagesUI.Contracts;

namespace ProductShopping.UI.RazorPagesUI.Pages;

public class OrdersModel(IOrdersApiClient ordersApiClient) : PageModel
{
    private readonly IOrdersApiClient _ordersApiClient = ordersApiClient;

    public IReadOnlyList<OrderDto> Orders { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 5;

    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var token = GetAccessToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var result = await _ordersApiClient.GetOrdersAsync(token, new PaginationParameters
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        }, ct);

        Orders = result.Data.ToList() ?? [];
        TotalCount = result.Metadata.TotalCount;
        TotalPages = result.Metadata.TotalPages;

        return Page();
    }
    public IActionResult OnPostCompletePayment(Guid orderId)
    {
        Console.WriteLine("OnPostCompletePayment Frontend");
        return RedirectToPage("/Orders");
    }

    public IEnumerable<int> GetVisiblePages()
    {
        if (TotalPages <= 0)
        {
            return Enumerable.Empty<int>();
        }

        const int maxVisiblePages = 5;
        var start = Math.Max(1, PageNumber - 2);
        var end = Math.Min(TotalPages, start + maxVisiblePages - 1);

        if (end - start + 1 < maxVisiblePages)
        {
            start = Math.Max(1, end - maxVisiblePages + 1);
        }

        return Enumerable.Range(start, end - start + 1);
    }

    public string GetStatusCssClass(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "pending" => "order-status-badge--pending",
            "paid" => "order-status-badge--paid",
            "payed" => "order-status-badge--paid",
            "processing" => "order-status-badge--processing",
            "shipped" => "order-status-badge--shipped",
            "completed" => "order-status-badge--completed",
            "cancelled" => "order-status-badge--cancelled",
            _ => "order-status-badge--default"
        };
    }

    private string? GetAccessToken()
    {
        return User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value
               ?? HttpContext.Session.GetString("AccessToken");
    }
}