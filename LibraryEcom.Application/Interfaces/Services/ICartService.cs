using LibraryEcom.Application.DTOs.Cart;

namespace LibraryEcom.Application.Interfaces.Services;

public interface ICartService
{
    List<CartDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<CartDto> GetAll(string? search = null);

    CartDto? GetById(Guid id);

    void Create(CreateCartDto dto);
    
    void Delete(Guid id);   
    
    void IncreaseQuantity(Guid bookId);
    
    void DecreaseQuantity(Guid bookId);
    
    Guid PlaceOrderFromCart();
    
    void CancelOrder(Guid orderId);
}