using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Domain.Common.Enum;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Application.DTOs.Book;

public class CreateBookDto
{
    public Guid Id { get; set; }

    public Guid PublisherId { get; set; }
    
    public string ISBN { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public  BookFormat BookFormat { get; set; } 
    
    public DateTime PublicationDate { get; set; }
    
    public Genre Genre { get; set; } 
    
    public decimal BasePrice { get; set; }
    
    public int PageCount { get; set; }
    
    public Language Language { get; set; } 
    
    public bool IsAvailable { get; set; }
    
    public DiscountDto Discount { get; set; }
}