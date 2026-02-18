using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Api.Constants;
using ProductShopping.Api.Contracts;
using ProductShopping.Api.DTOs.Payment;
using ProductShopping.Api.Models.Paging;
using ProductShopping.Api.Services;
using Stripe;
using Stripe.Checkout;

namespace ProductShopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IPaymentsService paymentsService, IConfiguration config) : ControllerBase
{
    [HttpPost("create-checkout-session")]
    [Authorize(Roles = RoleNames.User)]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] PaymentRequestDto request)
    {
        var result = await paymentsService.CreatePaymentSessionAsync(request);

        return Ok(new { Url = result.Url });
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
                config["Stripe:WebhookKey"],
                throwOnApiVersionMismatch: false
            );

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    var session = stripeEvent.Data.Object as Session;

                    int orderId;
                    int.TryParse(session.ClientReferenceId, out orderId);
                    var orderPayedResult = await paymentsService.SetOrderPayed(orderId);

                    if(!orderPayedResult.IsSuccess)
                    {
                        return NotFound(orderPayedResult.Errors[0].Description);
                    }

                    Console.WriteLine($"UDANA PŁATNOŚĆ! ID: {session.Id}, Kwota: {session.AmountTotal / 100m}, Email: {session.CustomerDetails.Email}");
                    Console.WriteLine($"ClientReferenceId(OrderId): {session.ClientReferenceId}");
                    break;

                case "checkout.session.expired":
                    var expired = stripeEvent.Data.Object as Session;
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


    [HttpGet("success")]
    public async Task<IActionResult> Success([FromQuery] string session_id)
    {
        if (string.IsNullOrEmpty(session_id))
            return BadRequest("Brak session_id");

        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(session_id);
            Console.WriteLine($"✅ Success redirect: {session.PaymentStatus}, ID: {session.Id}"); // Log w konsoli VS

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
    }
}
