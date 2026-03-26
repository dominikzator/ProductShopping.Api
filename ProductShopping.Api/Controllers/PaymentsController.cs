using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Contracts.Logging;
using Stripe;
using Stripe.Checkout;

namespace ProductShopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IPaymentsService paymentsService,
    IMailService mailService, IConfiguration config, IAppLogger<PaymentsController> logger) : ControllerBase
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

        logger.LogInformation("Stripe Webhook callback");

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
                        await paymentsService.SetOrderPayed(orderId);

                        logger.LogInformation($"Payment Successfull! ID: {session.Id}, Price: {session.AmountTotal / 100m}, Email: {session.CustomerDetails.Email}");

                        session.Metadata.TryGetValue("userEmail", out var userEmail);

                        await mailService.TrySendPaymentConfirmation(orderId, userEmail);

                        break;
                    }

                case "checkout.session.expired":
                    {
                        var expired = stripeEvent.Data.Object as Session;
                        logger.LogInformation($"Session Expired: {expired.Id}");

                        break;
                    }

                default:
                    {
                        logger.LogInformation($"Unhandled: {stripeEvent.Type}");

                        break;
                    }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            logger.LogInformation($"Stripe Error: {e.Message}");

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
            logger.LogInformation($"Error: {ex.Message}");

            return StatusCode(500, "Redirection Error");
        }
    }
}
