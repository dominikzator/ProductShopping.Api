using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using Stripe;
using Stripe.Checkout;

namespace ProductShopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IConfiguration _config;

    public PaymentsController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("create-checkout-session")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{request.Domain}/api/payments/success?session_id={{CHECKOUT_SESSION_ID}}", // Twoja strona sukcesu
            CancelUrl = $"{request.Domain}/api/payments/cancel", // Strona anulowania
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd", // Lub "pln"
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = request.ProductName ?? "Produkt",
                        Description = request.Description,
                    },
                    UnitAmount = (long)(request.Amount * 100), // np. 20.00 -> 2000
                },
                Quantity = 1,
            }
        },
            Mode = "payment",
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return Ok(new { Url = session.Url });
    }

    [HttpPost("stripe/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        Console.WriteLine("StripeWebhook!");

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _config["Stripe:WebhookKey"],
                throwOnApiVersionMismatch: false
            );

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    Console.WriteLine($"UDANA PŁATNOŚĆ! ID: {session.Id}, Kwota: {session.AmountTotal / 100m}, Email: {session.CustomerDetails.Email}");
                    break;

                case "checkout.session.expired":
                    var expired = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    Console.WriteLine($"SESJA WYGASŁA: {expired.Id}");
                    break;

                default:
                    Console.WriteLine($"Nieobsłużony: {stripeEvent.Type}");
                    break;
            }

            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine($"Stripe błąd: {e.Message}");
            return BadRequest(e.Message);
        }
    }


/*    [HttpGet("success")]
    public async Task<IActionResult> Success([FromQuery] string session_id)
    {
        if (string.IsNullOrEmpty(session_id))
            return BadRequest("Brak session_id");

        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(session_id);
            Console.WriteLine($"✅ Success redirect: {session.PaymentStatus}, ID: {session.Id}"); // Log w konsoli VS

            if (session.PaymentStatus == "paid")
            {
                // await UpdateOrderAsync(session.Id, true);
            }

            return Ok(new
            {
                Message = "Płatność udana!",
                Status = session.PaymentStatus,
                SessionId = session.Id,
                Amount = session.AmountTotal / 100m
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd success: {ex.Message}");
            return StatusCode(500, "Błąd przetwarzania");
        }
    }*/

/*    [HttpGet("cancel")]
    public IActionResult Cancel([FromQuery] string session_id = null)
    {
        Console.WriteLine($"❌ Cancel redirect: {session_id ?? "brak ID"}");
        return Ok(new { Message = "Płatność anulowana. Możesz spróbować ponownie." });
    }*/
}

public class CheckoutRequest
{
    public string Domain { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ProductName { get; set; }
    public string? Description { get; set; }
}