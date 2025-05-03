namespace LibraryEcom.Application.DTOs.Discounts;

public class UpdateDiscountDto
{
    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsSaleFlag { get; set; }

    public bool IsActive { get; set; } 
}
