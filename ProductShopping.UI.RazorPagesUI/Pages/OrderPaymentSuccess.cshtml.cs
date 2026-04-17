using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductShopping.Application.Features.Order.Queries.GetOrderDetails;
using ProductShopping.UI.RazorPagesUI.Contracts;
using SessionService = Stripe.Checkout.SessionService;

namespace ProductShopping.UI.RazorPagesUI.Pages
{
    public class OrderPaymentSuccessModel(IOrdersApiClient ordersApiClient) : PageModel
    {
        public string? SessionId { get; private set; }
        public string? OrderId {  get; private set; }
        public OrderDto Order {  get; private set; }

        public async Task<IActionResult> OnGetAsync(string? session_id)
        {
            SessionId = session_id;

            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(SessionId);

            var token = GetAccessToken();

            if (session.Metadata is null || !session.Metadata.TryGetValue("orderId", out var orderId) || string.IsNullOrWhiteSpace(orderId))
            {
                return Page();
            }

            OrderId = orderId;

            Order = await ordersApiClient.GetOrderAsync(token, int.Parse(orderId), CancellationToken.None);

            return Page();
        }

        private string? GetAccessToken() => User.FindFirst("access_token")?.Value;
    }
}
