using ProductShopping.Application.Results;

namespace ProductShopping.Application.Contracts
{
    public interface IMailService
    {
        Task<Result> SendEmailAsync(string email, string title, string description);
        Task<Result> TrySendPaymentConfirmation(int orderId, string? userEmail);
    }
}