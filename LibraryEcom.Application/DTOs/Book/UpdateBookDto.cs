using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Application.DTOs.Book;

public class UpdateBookDto
{
    public string ISBN { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public BookFormat BookFormat { get; set; }

    public DateTime PublicationDate { get; set; }

    public Genre Genre { get; set; }

    public decimal BasePrice { get; set; }

    public int PageCount { get; set; }

    public Language Language { get; set; }

    public bool IsAvailable { get; set; }
}