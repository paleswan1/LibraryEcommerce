namespace LibraryEcom.Application.DTOs.Discounts;

public class DiscountDto
{
    public Guid Id { get; set; }
    
    public Guid BookId { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsSaleFlag { get; set; }
}