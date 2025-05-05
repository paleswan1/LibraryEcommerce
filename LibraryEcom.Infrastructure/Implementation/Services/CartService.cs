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
                    PublisherId = book.PublisherId,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Description = book.Description,
                    BookFormat = book.BookFormat,
                    PublicationDate = book.PublicationDate,
                    Genre = book.Genre,
                    BasePrice = book.BasePrice,
                    PageCount = book.PageCount,
                    Language = book.Language,
                    IsAvailable = book.IsAvailable
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
                Book = new BookDto
                {
                    Id = book.Id,
                    PublisherId = book.PublisherId,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Description = book.Description,
                    BookFormat = book.BookFormat,
                    PublicationDate = book.PublicationDate,
                    Genre = book.Genre,
                    BasePrice = book.BasePrice,
                    PageCount = book.PageCount,
                    Language = book.Language,
                    IsAvailable = book.IsAvailable
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
                PublisherId = book.PublisherId,
                ISBN = book.ISBN,
                Title = book.Title,
                Description = book.Description,
                BookFormat = book.BookFormat,
                PublicationDate = book.PublicationDate,
                Genre = book.Genre,
                BasePrice = book.BasePrice,
                PageCount = book.PageCount,
                Language = book.Language,
                IsAvailable = book.IsAvailable
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

        var subtotal = cartItems.Sum(item =>
        {
            var book = genericRepository.GetById<Book>(item.BookId)
                       ?? throw new NotFoundException("Book not found while placing order");
            return book.BasePrice * item.Quantity;
        });

        decimal discount = 0;
        if (cartItems.Sum(x => x.Quantity) >= 5)
        {
            discount += subtotal * 0.05m;
        }

        var pastOrdersCount = genericRepository.Get<Order>(x => x.UserId == userId).Count();
        if (pastOrdersCount >= 10)
        {
            discount += subtotal * 0.10m;
        }

        var total = subtotal - discount;

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

        genericRepository.Insert(order);

        foreach (var item in cartItems)
        {
            var book = genericRepository.GetById<Book>(item.BookId)
                       ?? throw new NotFoundException("Book not found for order item");

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                BookId = item.BookId,
                Quantity = item.Quantity,
                UnitPrice = book.BasePrice
            };

            genericRepository.Insert(orderItem);
        }

        foreach (var item in cartItems)
        {
            genericRepository.Delete(item);
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
}
