using ProductShopping.Api.DTOs.Payment;
using ProductShopping.Api.Results;

namespace ProductShopping.Api.Contracts
{
    public interface IMailService
    {
        Task<Result> SendEmailAsync(string email, string title, string description);
        Task<Result> TrySendPaymentConfirmation(int orderId, string? userEmail);
    }
}