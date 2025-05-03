namespace LibraryEcom.Application.DTOs.Order;

public class UpdateOrderDto
{
    public DateTime? CompletionDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal LoyaltyDiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string ClaimCode { get; set; } = string.Empty;

    public DateTime? ClaimExpiry { get; set; }

    public bool IsClaimed { get; set; }
}