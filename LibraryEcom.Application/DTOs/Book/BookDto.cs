using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Application.DTOs.Review;
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
    
    public DateOnly PublicationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    public Genre Genre { get; set; } 
    
    public decimal BasePrice { get; set; }
    
    public int PageCount { get; set; }
    
    public Language Language { get; set; } 
    
    public bool IsAvailable { get; set; }
    
    public string? CoverImage { get; set; }
    
    public List<DiscountDto> Discount { get; set; } = new List<DiscountDto>();
    
    public DiscountDto ValidatedDiscount { get; set; }
    
    public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    
    public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    
    public List<BookAuthor> BookAuthors { get; set; } = new();

}