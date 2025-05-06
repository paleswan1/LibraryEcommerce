using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Domain.Common.Enum;
using LibraryEcom.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Application.DTOs.Book;

public class BookDto
{
    public Guid Id { get; set; }

    public string PublisherName { get; set; } = string.Empty;
    
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
    
    public string? CoverImage { get; set; }

    
    public List<DiscountDto> Discount { get; set; } = new List<DiscountDto>();
    
    public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    
    public List<BookAuthor> BookAuthors { get; set; } = new();

}