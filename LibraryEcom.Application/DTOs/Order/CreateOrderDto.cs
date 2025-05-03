namespace LibraryEcom.Application.DTOs.Order;

public class CreateOrderDto
{
    public Guid UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = "Pending";

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal LoyaltyDiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string ClaimCode { get; set; } = string.Empty;

    public DateTime? ClaimExpiry { get; set; }
}