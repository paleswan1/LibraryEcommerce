using LibraryEcom.Domain.Common.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LibraryEcom.Application.DTOs.Book;

public class UpdateBookDto
{
    public string ISBN { get; set; } = string.Empty;
    
    public string PublisherName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public BookFormat BookFormat { get; set; }

    public DateOnly PublicationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public Genre Genre { get; set; }

    public decimal BasePrice { get; set; }

    public int PageCount { get; set; }
    
    public bool IsAwarded { get; set; }
    
    public bool isFeatured { get; set; }
    
    public bool IsBestSeller { get; set; }

    public Language Language { get; set; }

    public bool IsAvailable { get; set; }

    public IFormFile? CoverImage { get; set; }

    public List<Guid> AuthorIds { get; set; } = new();
}