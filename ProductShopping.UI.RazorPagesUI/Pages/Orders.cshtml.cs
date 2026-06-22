using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProductShopping.Application.DTOs.Payment;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.Application.Models.Paging;
using ProductShopping.UI.Shared.Clients;
using ProductShopping.UI.Shared.Contracts;
using System.Security.Claims;

namespace ProductShopping.UI.RazorPagesUI.Pages;

public class OrdersModel(IOrdersApiClient ordersApiClient, IPaymentsApiClient paymentsApiClient) : PageModel
{
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
        var authRedirect = EnsureAuthenticated();
        if (authRedirect is not null)
        {
            return authRedirect;
        }

        await LoadOrdersAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnGetOrdersListAsync(CancellationToken ct)
    {
        var authRedirect = EnsureAuthenticated();
        if (authRedirect is not null)
        {
            return authRedirect;
        }

        await LoadOrdersAsync(ct);

        return new PartialViewResult
        {
            ViewName = "_OrdersListPartial",
            ViewData = new ViewDataDictionary<OrdersModel>(ViewData, this)
        };
    }

    public async Task<IActionResult> OnPostCompletePayment(int orderId, CancellationToken ct)
    {
        var token = GetAccessToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var order = await ordersApiClient.GetOrderAsync(token, orderId, ct);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        Console.WriteLine(baseUrl);

        var paymentRequestDto = new PaymentRequestDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            UserEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Domain = baseUrl,
            Items = order.OrderItems,
            TotalPrice = order.OrderItems.Sum(x => x.TotalPrice)
        };

        var result = await paymentsApiClient.CreatePaymentSessionAsync(token, paymentRequestDto, ct);

        return Redirect(result.PaymentUrl);
    }

    private async Task LoadOrdersAsync(CancellationToken ct)
    {
        var token = GetAccessToken();

        var result = await ordersApiClient.GetOrdersAsync(token!, new PaginationParameters
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        }, ct);

        Orders = result.Data.ToList() ?? [];
        TotalCount = result.Metadata.TotalCount;
        TotalPages = result.Metadata.TotalPages;
    }

    private IActionResult? EnsureAuthenticated()
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

        return null;
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
        return status?.Trim().ToLowerInvariant() switch
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