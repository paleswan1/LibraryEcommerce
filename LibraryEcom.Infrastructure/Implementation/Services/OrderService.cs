using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.Order;
using LibraryEcom.Application.DTOs.User;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class OrderService(IGenericRepository genericRepository,
    ICurrentUserService currentUserService) : IOrderService
{
    public List<OrderDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var orders = genericRepository.GetPagedResult<Order>(
            pageNumber,
            pageSize,
            out rowCount,
            x => string.IsNullOrEmpty(search) || x.Status.ToLower().Contains(search.ToLower()),
            x => x.OrderDate,
            false).ToList();

        return orders.Select(o => MapOrderToDto(o)).ToList();
    }

    public List<OrderDto> GetAll(string? search = null)
    {
        var orders = genericRepository.Get<Order>(
            x => string.IsNullOrEmpty(search) || x.Status.ToLower().Contains(search.ToLower())).ToList();

        return orders.Select(o => MapOrderToDto(o)).ToList();
    }

    public OrderDto? GetById(Guid id)
    {
        var order = genericRepository.GetById<Order>(id)
                    ?? throw new NotFoundException("Order not found");

        return MapOrderToDto(order);
    }

    public void Create(CreateOrderDto dto)
    {
        var user = genericRepository.GetById<User>(dto.UserId)
                   ?? throw new NotFoundException("User not found");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            OrderDate = dto.OrderDate,
            Status = dto.Status,
            Subtotal = dto.Subtotal,
            DiscountAmount = dto.DiscountAmount,
            LoyaltyDiscountAmount = dto.LoyaltyDiscountAmount,
            TotalAmount = dto.TotalAmount,
            ClaimCode = dto.ClaimCode,
            ClaimExpiry = dto.ClaimExpiry,
            IsClaimed = dto.
        };

        genericRepository.Insert(order);
    }

    public void Update(Guid id, UpdateOrderDto dto)
    {
        var order = genericRepository.GetById<Order>(id)
                    ?? throw new NotFoundException("Order not found");

        order.CompletionDate = dto.CompletionDate;
        order.Status = dto.Status;
        order.Subtotal = dto.Subtotal;
        order.DiscountAmount = dto.DiscountAmount;
        order.LoyaltyDiscountAmount = dto.LoyaltyDiscountAmount;
        order.TotalAmount = dto.TotalAmount;
        order.ClaimCode = dto.ClaimCode;
        order.ClaimExpiry = dto.ClaimExpiry;
        order.IsClaimed = dto.IsClaimed;

        genericRepository.Update(order);
    }

    public void Delete(Guid id)
    {
        var order = genericRepository.GetById<Order>(id)
                    ?? throw new NotFoundException("Order not found");

        genericRepository.Delete(order);
    }

    public Guid PlaceOrder()
    {
        var userId = currentUserService.GetUserId;

        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("User not found");

        var cartItems = genericRepository.Get<Cart>(x => x.UserId == userId).ToList();

        if (!cartItems.Any())
            throw new BadRequestException("Cart is empty.", new[] { "No items found in the cart." });

        var previousOrders = genericRepository
            .Get<Order>(x => x.UserId == userId, includeProperties: "OrderItems")
            .ToList();

        var orderedBookIds = previousOrders
            .SelectMany<Order, OrderItem>(o => o.OrderItems)
            .Select(oi => oi.BookId)
            .ToList();

        var discountPercentage = previousOrders.Count > 10
            ? 10
            : orderedBookIds.Count > 5
                ? 5
                : 0;

        var orderItems = new List<OrderItem>();

        foreach (var cart in cartItems)
        {
            var book = genericRepository.GetById<Book>(cart.BookId)
                       ?? throw new NotFoundException("Book not found");

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                Quantity = cart.Quantity,
                UnitPrice = book.BasePrice,
                OrderId = Guid.Empty // Will assign below
            });
        }

        var subtotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
        var discountAmount = subtotal * discountPercentage / 100;
        var grandTotal = subtotal - discountAmount;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            LoyaltyDiscountAmount = 0,
            TotalAmount = grandTotal,
            IsClaimed = false
        };

        var orderId = genericRepository.Insert(order);

        foreach (var item in orderItems)
        {
            item.OrderId = orderId;
            genericRepository.Insert(item);
        }

        genericRepository.RemoveMultipleEntity(cartItems);

        return orderId;
    }

    public void CancelOrder(Guid orderId)
    {
        var userId = currentUserService.GetUserId;

        var order = genericRepository.GetById<Order>(orderId)
                     ?? throw new NotFoundException("Order not found");

        if (order.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to cancel this order.");

        if (string.IsNullOrWhiteSpace(order.Status))
            throw new BadRequestException("Order status is invalid.", new[] { "Status cannot be empty or null." });

        if (order.Status != "Pending")
            throw new BadRequestException("Only pending orders can be cancelled.", new[] { "The order cannot be cancelled because it is not in 'Pending' status." });

        genericRepository.Delete(order);
    }

    private static OrderDto MapOrderToDto(Order o)
    {
        return new OrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            OrderDate = o.OrderDate,
            CompletionDate = o.CompletionDate,
            Status = o.Status,
            Subtotal = o.Subtotal,
            DiscountAmount = o.DiscountAmount,
            LoyaltyDiscountAmount = o.LoyaltyDiscountAmount,
            TotalAmount = o.TotalAmount,
            ClaimCode = o.ClaimCode,
            ClaimExpiry = o.ClaimExpiry,
            IsClaimed = o.IsClaimed
        };
    }
}
