using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;
using LibraryEcom.Domain.Common.Enum;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Domain.Entities;

public class Book: BaseEntity<Guid>
{
    public string PublisherName { get; set; } = string.Empty;
    
    public string ISBN { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public BookFormat BookFormat { get; set; } 
    
    public DateOnly PublicationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    public Genre Genre { get; set; } 
    
    public decimal BasePrice { get; set; }
    
    public int PageCount { get; set; }
    
    public Language Language { get; set; } 
    
    public bool IsAvailable { get; set; }
    
    public string? CoverImage { get; set; }
    
    public bool IsAwarded { get; set; }
    
    public bool isFeatured { get; set; }
    
    public bool IsBestSeller { get; set; }
    
    public virtual ICollection<Review> Reviews { get; set; } = [];

    
    public virtual ICollection<Discount> Discount { get; set; } = [];
    
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = [];

    
}