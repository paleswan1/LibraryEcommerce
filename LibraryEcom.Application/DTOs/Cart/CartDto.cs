using LibraryEcom.Application.DTOs.Book;

namespace LibraryEcom.Application.DTOs.Cart;

public class CartDto
{
    public Guid Id { get; set; }

    public BookDto Book { get; set; }
    
    public int Quantity { get; set; }
}