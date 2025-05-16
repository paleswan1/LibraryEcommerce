using LibraryEcom.Application.DTOs.Book;

namespace LibraryEcom.Application.DTOs.Cart;

public class CreateCartDto
{
    public Guid BookId { get; set; }
}