namespace ProductShopping.UI.Shared.DTOs.Payments;

/// <summary>
/// Odpowiedź z GET api/payments/success?session_id=... (status sesji Stripe Checkout).
/// </summary>
public sealed class PaymentSessionStatusDto
{
    public bool Success { get; set; }
    public string? SessionId { get; set; }

    /// <summary>Status sesji Stripe: "open", "complete" lub "expired".</summary>
    public string? Status { get; set; }

    /// <summary>Status płatności Stripe: "paid", "unpaid" lub "no_payment_required".</summary>
    public string? PaymentStatus { get; set; }

    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? OrderId { get; set; }

    public bool IsPaid => string.Equals(PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);

    public int? OrderIdValue => int.TryParse(OrderId, out var id) ? id : null;
}
