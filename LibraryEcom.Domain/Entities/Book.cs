using LibraryEcom.Domain.Common.Base;
using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Domain.Entities;

public class Book: BaseEntity<Guid>
{
    
    public Guid PublisherId { get; set; }
    
    public string ISBN { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public BookFormat BookFormat { get; set; } 
    
    public DateTime PublicationDate { get; set; }
    
    public Genre Genre { get; set; } 
    
    public decimal BasePrice { get; set; }
    
    public int PageCount { get; set; }
    
    public Language Language { get; set; } 
    
    public bool IsAvailable { get; set; }
    
    public Discount Discount { get; set; }
    
    // public List<BookAuthor> BookAuthors { get; set; } = new();

}