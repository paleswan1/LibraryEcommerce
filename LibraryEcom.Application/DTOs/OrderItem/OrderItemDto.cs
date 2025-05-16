using LibraryEcom.Application.DTOs.Book;

namespace LibraryEcom.Application.DTOs.OrderItem;

public class OrderItemDto
{
    public Guid Id { get; set; }
    
    public Guid BookId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public BookDto Book { get; set; }
    
}