using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.Whitelist;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class WhitelistService(
    IGenericRepository genericRepository,
    ICurrentUserService currentUserService) : IWhitelistService
{
    public List<WhiteListDto> GetUserWishlist()
    {
        var userId = currentUserService.GetUserId;

        var whitelistItems = genericRepository.Get<WhiteList>(x => x.UserId == userId).ToList();

        var bookIds = whitelistItems.Select(x => x.BookId).Distinct().ToList();
        var books = genericRepository.Get<Book>(x => bookIds.Contains(x.Id)).ToList();

        var result = new List<WhiteListDto>();

        foreach (var item in whitelistItems)
        {
            var book = books.FirstOrDefault(b => b.Id == item.BookId);
            result.Add(new WhiteListDto
            {
                Id = item.Id,
                BookId = item.BookId,
                CreatedAt = item.CreatedAt,
                Book = book != null ? new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Description = book.Description,
                    BasePrice = book.BasePrice,
                    Language = book.Language,
                    BookFormat = book.BookFormat,
                    Genre = book.Genre,
                    PublicationDate = book.PublicationDate,
                    IsAvailable = book.IsAvailable,
                    PageCount = book.PageCount,
                    PublisherName = book.PublisherName,
                } : null
            });
        }
        return result;
    }

    public List<WhiteListDto> GetUserWishlist(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var userId = currentUserService.GetUserId;

        var pagedItems = genericRepository.GetPagedResult<WhiteList>(
            pageNumber,
            pageSize,
            out rowCount,
            x => x.UserId == userId)
            .ToList();

        var bookIds = pagedItems.Select(x => x.BookId).Distinct().ToList();
        var books = genericRepository.Get<Book>(x => bookIds.Contains(x.Id)).ToList();

        var result = new List<WhiteListDto>();

        foreach (var item in pagedItems)
        {
            var book = books.FirstOrDefault(b => b.Id == item.BookId);
            if (book != null && (string.IsNullOrEmpty(search) || book.Title.ToLower().Contains(search.ToLower())))
            {
                result.Add(new WhiteListDto
                {
                    Id = item.Id,
                    BookId = item.BookId,
                    CreatedAt = item.CreatedAt,
                    Book = new BookDto
                    {
                        Id = book.Id,
                        Title = book.Title,
                        ISBN = book.ISBN,
                        Description = book.Description,
                        BasePrice = book.BasePrice,
                        Language = book.Language,
                        BookFormat = book.BookFormat,
                        Genre = book.Genre,
                        PublicationDate = book.PublicationDate,
                        IsAvailable = book.IsAvailable,
                        PageCount = book.PageCount,
                        PublisherName = book.PublisherName,
                    }
                });
            }
        }

        return result;
    }

    public void AddToWishlist(CreateWhiteListDto dto)
    {
        var userId = currentUserService.GetUserId;

        var exists = genericRepository.Exists<WhiteList>(x => x.UserId == userId && x.BookId == dto.BookId);

        if (exists)
            throw new BadRequestException("Book is already in your wishlist.",
                ["The following user is not active, please contact the administrator"]);

        var entity = new WhiteList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = dto.BookId,
            CreatedAt = DateTime.UtcNow
        };

        genericRepository.Insert(entity);
    }

    public void RemoveFromWishlist(Guid whiteListId)
    {
        var userId = currentUserService.GetUserId;

        var item = genericRepository.GetById<WhiteList>(whiteListId)
                   ?? throw new NotFoundException("Wishlist item not found.");

        if (item.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to remove this item.");

        genericRepository.Delete(item);
    }

    public bool IsBookWishlisted(Guid bookId)
    {
        var userId = currentUserService.GetUserId;

        return genericRepository.Exists<WhiteList>(x => x.UserId == userId && x.BookId == bookId);
    }
}
