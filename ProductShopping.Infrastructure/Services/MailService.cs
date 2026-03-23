using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductShopping.Application.Contracts;
using ProductShopping.Application.Results;
using ProductShopping.Domain.Models;
using ProductShopping.Persistence.DatabaseContext;
using System.Text;

namespace ProductShopping.Application.Services;

public class MailService(ProductShoppingDbContext context, IIdentityUserService identityUserService, ILogger<MailService> logger, IHttpContextAccessor httpContextAccessor, IConfiguration config) : IMailService
{
    public async Task<Result> SendEmailAsync(string email, string title, string description)
    {
        logger.LogInformation($"SendEmailAsync to userEmail: {email}");

        string communicationConnectionString = config.GetConnectionString("CommunicationServicesConnectionString");

        var data = new EmailData
        {
            To = email,
            Subject = title,
            Message = description
        };

        var emailContent = new EmailContent(data.Subject)
        {
            PlainText = data.Message
        };

        var emailRecipients = new EmailRecipients(new List<EmailAddress> { new(data.To) });

        var emailMessage = new EmailMessage(
            senderAddress: data.From,
            recipients: emailRecipients,
            content: emailContent);

        try
        {
            var emailClient = new EmailClient(communicationConnectionString);
            var operation = await emailClient.SendAsync(WaitUntil.Started, emailMessage);

            do
            {
                await Task.Delay(500);
                await operation.UpdateStatusAsync();
            } while (!operation.HasCompleted);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("ex.Message: " + ex.Message);

            return Result.Failure();
        }

        logger.LogInformation("Email Successfully Sent");
        return Result.Success();
    }

    public async Task<Result> TrySendPaymentConfirmation(int orderId, string? userEmail)
    {
        try
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            var userId = await identityUserService.GetUserIdFromOrderAsync(order);

            var emailConfirmed = await identityUserService.IsEmailConfirmedAsync(userId);
            logger.LogInformation($"Emailconfirmed: " + emailConfirmed);
            if (emailConfirmed)
            {
                await SendPaymentConfirmationEmail(orderId, userEmail);

                return Result.Success();
            }

            logger.LogInformation("There was no exception, but the email was not sent");
            return Result.Failure(new Error("123", $"There was no exception, but the email was not sent"));
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Confirmation Email was not sent. Exception message: {ex.Message}");
            return Result.Failure(new Error("123", $"Confirmation Email was not sent. Exception message: {ex.Message}"));
        }
    }

    private async Task<Result> SendPaymentConfirmationEmail(int orderId, string userEmail)
    {
        var order = await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);

        var subject = $"Hello, we have received your payment for order: {order.OrderNumber}";
        var message = GetItemListings(order.OrderItems);

        try
        {
            await SendEmailAsync(userEmail, subject, message);

            return Result.Success();
        }
        catch (Exception ex)
        {
            
        }

        return Result.Failure();
    }

    private string GetItemListings(List<OrderItem> orderItems)
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Ordered Items:");
        foreach (OrderItem item in orderItems)
        {
            stringBuilder.AppendLine($"{item.Quantity}x {item.ProductName}, Price: {item.Quantity * item.UnitPrice}");
        }
        stringBuilder.AppendLine($"Total Order Price: {orderItems.Sum(orderItem => orderItem.TotalPrice)}");

        return stringBuilder.ToString();
    }
}

public class EmailData
{
    public string To { get; set; }
    public string From { get; set; } = "donotreply@8044aea5-4316-4b2f-849c-7afa34f35f40.azurecomm.net";
    public string Subject { get; set; }
    public string Message { get; set; }
}