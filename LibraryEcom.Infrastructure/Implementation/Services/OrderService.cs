using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.Email;
using LibraryEcom.Application.DTOs.Order;
using LibraryEcom.Application.DTOs.User;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Domain.Entities.Identity;
using MailKit;
using System.Globalization;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.DTOs.OrderItem;
using LibraryEcom.Application.Hubs;
using LibraryEcom.Domain.Common.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class OrderService(
    IGenericRepository genericRepository,
    ICurrentUserService currentUserService,
    IEmailService emailService,
    IWebHostEnvironment env,
    IHubContext<NotificationsHub, INotificationsClient> hubContext) : IOrderService


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
            IsClaimed = false,
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

    public async Task<Guid> PlaceOrder()
    {
        var userId = currentUserService.GetUserId;
        var user = genericRepository.GetById<User>(userId) ?? throw new NotFoundException("User not found");

        var cartItems = genericRepository.Get<Cart>(x => x.UserId == userId).ToList();
        if (!cartItems.Any())
            throw new BadRequestException("Cart is empty.", new[] { "No items found in the cart." });

        var previousOrders = genericRepository.Get<Order>(
            x => x.UserId == userId && x.Status == "Completed", includeProperties: "OrderItems").ToList();

        var orderedBookIds = previousOrders.SelectMany(o => o.OrderItems).Select(oi => oi.BookId).ToList();

        var discountPercentage = 0;
        if (orderedBookIds.Count >= 5) discountPercentage += 5;
        if (previousOrders.Count >= 10) discountPercentage += 10;

        var orderItems = new List<OrderItem>();
        foreach (var cart in cartItems)
        {
            var book = genericRepository.GetById<Book>(cart.BookId) ?? throw new NotFoundException("Book not found");
            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                Quantity = cart.Quantity,
                UnitPrice = book.BasePrice,
                OrderId = Guid.Empty
            });
        }

        var subtotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
        var discountAmount = subtotal * discountPercentage / 100;
        var claimCode = $"CLM-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            TotalAmount = subtotal - discountAmount,
            ClaimCode = claimCode,
            ClaimExpiry = DateTime.UtcNow.AddDays(7),
            IsClaimed = false
        };

        var orderId = genericRepository.Insert(order);

        foreach (var item in orderItems)
        {
            item.OrderId = orderId;
            genericRepository.Insert(item);
        }

        genericRepository.RemoveMultipleEntity(cartItems);

        var templatePath = Path.Combine(env.WebRootPath, "email-templates", "ClaimCodeTemplate.html");
        var template = File.ReadAllText(templatePath);

        var body = template
            .Replace("{{FullName}}", user.Name)
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{ClaimCode}}", claimCode)
            .Replace("{{ClaimExpiry}}", order.ClaimExpiry?.ToString("yyyy-MM-dd") ?? "")
            .Replace("{{UserId}}", user.Id.ToString())
            .Replace("{{TotalAmount}}", order.TotalAmount.ToString("F2"))
            .Replace("{{DiscountedAmount}}", (order.DiscountAmount + order.LoyaltyDiscountAmount).ToString("F2"));


        if (string.IsNullOrWhiteSpace(user.Email))
            throw new BadRequestException("User email is missing. Cannot send order confirmation.", ["Check"]);

        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email,
            Subject = "📦 Your Order & Claim Code",
            EmailProcess = EmailProcess.OrderConfirmation,
            PrimaryMessage = $"{claimCode}",
            UserName = user.UserName ?? "",
            Remarks = $"Order #{order.Id}",
            Body = body,
            IsHtml = true
        };

        await emailService.SendEmail(emailDto);

        var orderedBookTitles = orderItems
            .Select(i => genericRepository.GetById<Book>(i.BookId)?.Title)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();

        await hubContext.Clients.User(userId.ToString())
            .ReceiveNotification(
                $"📦 Hi {user.Name}, your order has been placed successfully for: {string.Join(", ", orderedBookTitles)}");
        return orderId;
    }

    public async Task<Guid> FulfillOrderByClaimCode(string claimCode)
    {
        var order = genericRepository.GetFirstOrDefault<Order>(x =>
                        x.ClaimCode == claimCode && !x.IsClaimed && x.Status == "Pending")
                    ?? throw new NotFoundException("Invalid or already claimed code.");

        order.IsClaimed = true;
        order.Status = "Completed";
        order.CompletionDate = DateTime.UtcNow;

        genericRepository.Update(order);

        var user = genericRepository.GetById<User>(order.UserId)
                   ?? throw new NotFoundException("User not found");

        var templatePath = Path.Combine(env.WebRootPath, "email-templates", "OrderFulfilledTemplate.html");
        var template = File.ReadAllText(templatePath);

        var body = template
            .Replace("{{FullName}}", user.Name)
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{CompletionDate}}", order.CompletionDate?.ToString("yyyy-MM-dd") ?? "");

        var emailDto = new EmailDto
        {
            FullName = user.Name,
            ToEmailAddress = user.Email,
            Subject = "✅ Your Order Has Been Fulfilled",
            EmailProcess = EmailProcess.OrderFulfillment,
            PrimaryMessage = $"Order #{order.Id} has been successfully fulfilled.",
            UserName = user.UserName ?? "",
            Remarks = $"Order #{order.Id} fulfilled on {order.CompletionDate?.ToString("yyyy-MM-dd")}",
            Body = body,
            IsHtml = true
        };

        await emailService.SendEmail(emailDto);

        await hubContext.Clients.User(user.Id.ToString())
            .ReceiveNotification($"✅ Hi {user.Name}, your order #{order.Id} has been successfully fulfilled.");

        return order.Id;
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
            throw new BadRequestException("Only pending orders can be cancelled.",
                new[] { "The order cannot be cancelled because it is not in 'Pending' status." });

        genericRepository.Delete(order);
    }

    public List<OrderDto> GetMyOrders()
    {
        var userId = currentUserService.GetUserId;

        var orders = genericRepository.Get<Order>(x => x.UserId == userId)
            .OrderByDescending(x => x.OrderDate)
            .ToList();

        return orders.Select(o => MapOrderToDto(o)).ToList();
    }

    private OrderDto MapOrderToDto(Order o)
    {
        var orderItems = genericRepository.Get<OrderItem>(x => x.OrderId == o.Id).ToList();

        var bookIds = orderItems.Select(i => i.BookId).Distinct().ToList();
        var books = genericRepository.Get<Book>(x => bookIds.Contains(x.Id)).ToList();

        var items = orderItems.Select(i =>
        {
            var book = books.FirstOrDefault(b => b.Id == i.BookId);
            return new OrderItemDto
            {
                Id = i.Id,
                BookId = i.BookId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Book = book == null
                    ? null
                    : new BookDto
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Genre = book.Genre,
                        CoverImage = book.CoverImage,
                        BasePrice = book.BasePrice,
                        PublisherName = book.PublisherName,
                        ISBN = book.ISBN,
                        Language = book.Language,
                        Description = book.Description,
                        BookFormat = book.BookFormat,
                        PageCount = book.PageCount,
                        PublicationDate = book.PublicationDate,
                        IsAvailable = book.IsAvailable
                    }
            };
        }).ToList();

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
            IsClaimed = o.IsClaimed,
            Items = items
        };
    }
}