namespace LibraryEcom.Application.DTOs.Book;

public class PagedBookResponseDto
{
    public List<BookDto> Result { get; set; } = new();
    
    public int TotalCount { get; set; }
    
    public int CurrentPage { get; set; }
    
    public int TotalPages { get; set; }
    
    public int PageSize { get; set; }
}
