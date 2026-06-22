using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.UI.Shared.Contracts;
using SessionService = Stripe.Checkout.SessionService;

namespace ProductShopping.UI.RazorPagesUI.Pages
{
    public class OrderPaymentCancelModel(IOrdersApiClient ordersApiClient) : PageModel
    {
        public string? SessionId { get; private set; }
        public string? OrderId { get; private set; }
        public OrderDto? Order { get; private set; }

        public async Task<IActionResult> OnGetAsync(string? session_id)
        {
            if (string.IsNullOrWhiteSpace(session_id))
                return Page();

            SessionId = session_id;

            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(session_id);

            if (session.Metadata is null ||
                !session.Metadata.TryGetValue("orderId", out var orderId) ||
                string.IsNullOrWhiteSpace(orderId))
            {
                return Page();
            }

            OrderId = orderId;

            var token = GetAccessToken();

            if (string.IsNullOrWhiteSpace(token))
                return Page();

            Order = await ordersApiClient.GetOrderAsync(token, int.Parse(orderId), CancellationToken.None);

            return Page();
        }

        private string? GetAccessToken()
            => User.FindFirst("access_token")?.Value;
    }
}