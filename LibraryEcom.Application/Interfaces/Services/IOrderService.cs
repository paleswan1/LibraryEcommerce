using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Order;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IOrderService : ITransientService
{
    List<OrderDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<OrderDto> GetAll(string? search = null);

    OrderDto? GetById(Guid id);

    void Create(CreateOrderDto dto);

    void Update(Guid id, UpdateOrderDto dto);

    void Delete(Guid id);

    Task<Guid> PlaceOrder();

    Task<Guid> FulfillOrderByClaimCode(string claimCode);

    void CancelOrder(Guid orderId);
    
    List<OrderDto> GetMyOrders();
}