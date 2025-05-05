namespace LibraryEcom.Application.DTOs.Discounts;

public class UpdateDiscountDto
{
    public decimal DiscountPercentage { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsSaleFlag { get; set; }

    public bool IsActive { get; set; } 
}
