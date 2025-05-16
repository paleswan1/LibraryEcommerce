namespace LibraryEcom.Application.DTOs.Discounts;

public class DiscountDto
{
    public Guid Id { get; set; }
    
    public Guid BookId { get; set; }
    
    public string BookTitle { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public bool IsSaleFlag { get; set; }
}