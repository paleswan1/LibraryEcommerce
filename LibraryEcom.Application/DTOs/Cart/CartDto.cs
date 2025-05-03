using LibraryEcom.Application.DTOs.Book;

namespace LibraryEcom.Application.DTOs.Cart;

public class CartDto
{
    public BookDto Book { get; set; }
    
    public int Quantity { get; set; }
}