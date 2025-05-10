using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.DTOs.Cart;
using LibraryEcom.Application.DTOs.Order;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class CartService(
    IGenericRepository genericRepository,
    ICurrentUserService currentUserService) : ICartService
{
    public List<CartDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var userId = currentUserService.GetUserId;

        var carts = genericRepository.GetPagedResult<Cart>(
            pageNumber,
            pageSize,
            out rowCount,
            x => x.UserId == userId).ToList();

        return carts.Select(cart =>
        {
            var book = genericRepository.GetById<Book>(cart.BookId)
                       ?? throw new NotFoundException("Book not found");

            return new CartDto
            {
                Id = cart.Id,
                Book = new BookDto
                {
                    Id = book.Id,
                    PublisherName = book.PublisherName,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Description = book.Description,
                    BookFormat = book.BookFormat,
                    PublicationDate = book.PublicationDate,
                    Genre = book.Genre,
                    BasePrice = book.BasePrice,
                    PageCount = book.PageCount,
                    Language = book.Language,
                    IsAvailable = book.IsAvailable,
                    CoverImage = book.CoverImage

                },
                Quantity = cart.Quantity
            };
        }).ToList();
    }

    public List<CartDto> GetAll(string? search = null)
    {
        var userId = currentUserService.GetUserId;

        var carts = genericRepository.Get<Cart>(x => x.UserId == userId).ToList();

        return carts.Select(cart =>
        {
            var book = genericRepository.GetById<Book>(cart.BookId)
                       ?? throw new NotFoundException("Book not found");

            return new CartDto
            {
                Id = cart.Id,
                Book = new BookDto
                {
                    Id = book.Id,
                    PublisherName = book.PublisherName,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Description = book.Description,
                    BookFormat = book.BookFormat,
                    PublicationDate = book.PublicationDate,
                    Genre = book.Genre,
                    BasePrice = book.BasePrice,
                    PageCount = book.PageCount,
                    Language = book.Language,
                    IsAvailable = book.IsAvailable,
                    CoverImage = book.CoverImage

                },
                Quantity = cart.Quantity
            };
        }).ToList();
    }

    public CartDto? GetById(Guid id)
    {
        var cart = genericRepository.GetById<Cart>(id)
                   ?? throw new NotFoundException("Cart item not found.");

        var book = genericRepository.GetById<Book>(cart.BookId)
                   ?? throw new NotFoundException("Book not found");

        return new CartDto
        {
            Book = new BookDto
            {
                Id = book.Id,
                PublisherName = book.PublisherName,
                ISBN = book.ISBN,
                Title = book.Title,
                Description = book.Description,
                BookFormat = book.BookFormat,
                PublicationDate = book.PublicationDate,
                Genre = book.Genre,
                BasePrice = book.BasePrice,
                PageCount = book.PageCount,
                Language = book.Language,
                IsAvailable = book.IsAvailable,
                CoverImage = book.CoverImage
            },
            Quantity = cart.Quantity
        };
    }

    public void Create(CreateCartDto dto)
    {
        var userId = currentUserService.GetUserId;

        var book = genericRepository.GetById<Book>(dto.BookId)
                   ?? throw new NotFoundException("Book not found");

        var existingCart = genericRepository.Get<Cart>(x => x.UserId == userId && x.BookId == dto.BookId).FirstOrDefault();

        if (existingCart != null)
        {
            existingCart.Quantity += 1;
            genericRepository.Update(existingCart);
        }
        else
        {
            var newCart = new Cart
            {
                Id = Guid.NewGuid(),
                BookId = dto.BookId,
                Quantity = 1,
                UserId = userId
            };

            genericRepository.Insert(newCart);
        }
    }

    public void Delete(Guid id)
    {
        var userId = currentUserService.GetUserId;

        var cart = genericRepository.GetById<Cart>(id)
                   ?? throw new NotFoundException("Cart item not found.");

        if (cart.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to delete this cart item.");

        genericRepository.Delete(cart);
    }

    public void IncreaseQuantity(Guid bookId)
    {
        var userId = currentUserService.GetUserId;

        var cart = genericRepository.Get<Cart>(x => x.BookId == bookId && x.UserId == userId).FirstOrDefault()
                   ?? throw new NotFoundException("Cart item not found.");

        cart.Quantity += 1;
        genericRepository.Update(cart);
    }

    public void DecreaseQuantity(Guid bookId)
    {
        var userId = currentUserService.GetUserId;

        var cart = genericRepository.Get<Cart>(x => x.BookId == bookId && x.UserId == userId).FirstOrDefault()
                   ?? throw new NotFoundException("Cart item not found.");

        if (cart.Quantity <= 1)
        {
            genericRepository.Delete(cart);
        }
        else
        {
            cart.Quantity -= 1;
            genericRepository.Update(cart);
        }
    }

    public Guid PlaceOrderFromCart()
{
    var userId = currentUserService.GetUserId;

    var cartItems = genericRepository.Get<Cart>(x => x.UserId == userId).ToList();

    if (!cartItems.Any())
        throw new BadRequestException("Cart is empty.", new[] { "No items found in the cart." });

    List<OrderItem> orderItems = new();
    decimal subtotal = 0;

    try
    {
        foreach (var cart in cartItems)
        {
            var book = genericRepository.GetById<Book>(cart.BookId)
                       ?? throw new NotFoundException($"Book not found for BookId: {cart.BookId}");

            subtotal += book.BasePrice * cart.Quantity;

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                Quantity = cart.Quantity,
                UnitPrice = book.BasePrice
                // OrderId will be set after order is created
            });
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Error preparing order items: " + (ex.InnerException?.Message ?? ex.Message));
    }

    // Apply discount rules
    decimal discount = 0;
    int totalBooksOrdered = cartItems.Sum(x => x.Quantity);
    if (totalBooksOrdered >= 5)
    {
        discount += subtotal * 0.05m;
    }

    int pastOrdersCount = genericRepository.Get<Order>(x => x.UserId == userId).Count();
    if (pastOrdersCount >= 10)
    {
        discount += subtotal * 0.10m;
    }

    decimal total = subtotal - discount;

    var order = new Order
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        OrderDate = DateTime.UtcNow,
        Status = "Pending",
        Subtotal = subtotal,
        DiscountAmount = discount,
        LoyaltyDiscountAmount = 0,
        TotalAmount = total,
        IsClaimed = false
    };

    try
    {
        // Save order
        genericRepository.Insert(order);

        // Link and save each order item
        foreach (var item in orderItems)
        {
            item.OrderId = order.Id;
            genericRepository.Insert(item);
        }

        // Clear the cart
        genericRepository.RemoveMultipleEntity(cartItems);
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to place order: " + (ex.InnerException?.Message ?? ex.Message));
    }

    return order.Id;
}


    public void CancelOrder(Guid orderId)
    {
        var order = genericRepository.GetById<Order>(orderId)
                     ?? throw new NotFoundException("Order not found");

        if (order.Status != "Pending")
            throw new BadRequestException("Only pending orders can be cancelled.", new[] { "The order cannot be cancelled because it is not in 'Pending' status." });

        genericRepository.Delete(order);
    }
    
    public void ClearCart()
    {
        var userId = currentUserService.GetUserId;

        var cartItems = genericRepository.Get<Cart>(x => x.UserId == userId).ToList();

        foreach (var item in cartItems)
        {
            genericRepository.Delete(item);
        }
    }

}
