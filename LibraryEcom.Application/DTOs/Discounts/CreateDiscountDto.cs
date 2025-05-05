namespace LibraryEcom.Application.DTOs.Discount;

public class CreateDiscountDto
{
    public Guid BookId { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsSaleFlag { get; set; }
    
}