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
    /// <summary>
    /// Anonymous webhook endpoint which is beeing called after successful payment. Changes Order Status to Payed. This shouldn't be called manually.
    /// </summary>
    /// <returns></returns>
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
                    {
                        var session = stripeEvent.Data.Object as Session;

                        int orderId;
                        int.TryParse(session.ClientReferenceId, out orderId);
                        var orderPayedResult = await paymentsService.SetOrderPayed(orderId);

                        if (!orderPayedResult.IsSuccess)
                        {
                            return NotFound(orderPayedResult.Errors[0].Description);
                        }
                        Console.WriteLine($"Payment Successfull! ID: {session.Id}, Price: {session.AmountTotal / 100m}, Email: {session.CustomerDetails.Email}");
                        break;
                    }


                case "checkout.session.expired":
                    {
                        var expired = stripeEvent.Data.Object as Session;
                        Console.WriteLine($"Session Expired: {expired.Id}");
                        break;
                    }

                default:
                    {
                        Console.WriteLine($"Unhandled: {stripeEvent.Type}");
                        break;
                    }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine($"Stripe Error: {e.Message}");
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Anonymous redirection endpoint after successfull payment that is displaying Success info.
    /// </summary>
    /// <param name="session_id"></param>
    /// <returns></returns>
    [HttpGet("success")]
    public async Task<IActionResult> Success([FromQuery] string session_id)
    {
        if (string.IsNullOrEmpty(session_id))
            return BadRequest("No session_id");

        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(session_id);

            return Ok(new
            {
                Message = "Payment successfull!!!",
                Status = session.PaymentStatus,
                SessionId = session.Id,
                Amount = session.AmountTotal / 100m
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error success: {ex.Message}");
            return StatusCode(500, "Redirection Error");
        }
    }
}
