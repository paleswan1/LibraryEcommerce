using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.DTOs.User;

namespace LibraryEcom.Application.DTOs.Whitelist;

public class WhiteListDto
{
    public Guid Id { get; set; }
    
    public Guid BookId { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserDto? User { get; set; }

    public BookDto? Book { get; set; }
}