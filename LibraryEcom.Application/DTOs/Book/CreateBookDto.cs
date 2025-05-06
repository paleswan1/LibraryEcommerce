using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Domain.Common.Enum;
using LibraryEcom.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Application.DTOs.Book;

public class CreateBookDto
{
    public Guid Id { get; set; }

    public string PublisherName { get; set; } = string.Empty;
    
    public List<Guid> AuthorIds { get; set; } = new();
    
    public string ISBN { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public  BookFormat BookFormat { get; set; } 
    
    public DateOnly PublicationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    public Genre Genre { get; set; } 
    
    public decimal BasePrice { get; set; }
    
    public int PageCount { get; set; }
    
    public Language Language { get; set; } 
    
    public bool IsAvailable { get; set; }
    
    public IFormFile? CoverImage { get; set; }

    
    // public BookAuthor Author { get; set; }
}