using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class BookService(IGenericRepository genericRepository) : IBookService
{
    public List<BookDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var books = genericRepository.GetPagedResult<Book>(
            pageNumber,
            pageSize,
            out rowCount,
            x => (string.IsNullOrEmpty(search)
                 || x.Title.ToLower().Contains(search.ToLower())
                 || x.Description.ToLower().Contains(search.ToLower()))
                 && (isActive == null || x.IsAvailable == isActive))
        .ToList();

    var bookIds = books.Select(b => b.Id).ToList();

    var bookAuthors = genericRepository.Get<BookAuthor>(ba => bookIds.Contains(ba.BookId))
        .Include(ba => ba.Author)
        .ToList();
    

    var bookDtos = new List<BookDto>();

    foreach (var book in books)
    {
        var authors = bookAuthors
            .Where(ba => ba.BookId == book.Id)
            .Select(ba => new AuthorDto
            {
                Id = ba.Author.Id,
                Name = ba.Author.Name,
                Biography = ba.Author.Biography,
                BirthDate = ba.Author.BirthDate
            }).ToList();

        bookDtos.Add(new BookDto
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
            IsAvailable = book.IsAvailable,
            // Discount = book.Discount != null
            //     ? new DiscountDto
            //     {
            //         Id = book.Discount.Id,
            //         BookId = book.Discount.BookId,
            //         DiscountPercentage = book.Discount.DiscountPercentage,
            //         StartDate = book.Discount.StartDate,
            //         EndDate = book.Discount.EndDate,
            //         IsActive = book.Discount.IsActive,
            //         IsSaleFlag = book.Discount.IsSaleFlag
            //     }
            //     : null,
            Authors = authors
        });
    }

    return bookDtos;
    }

    public List<BookDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null,
        string? sortBy = null, bool isDescending = false)
    {
        throw new NotImplementedException();
    }

    public List<BookDto> GetAll(string? search = null)
    {
        var books = genericRepository.Get<Book>(
            x => string.IsNullOrEmpty(search)
                 || x.Title.ToLower().Contains(search.ToLower())
                 || x.Description.ToLower().Contains(search.ToLower()))
            .ToList();

        var bookIds = books.Select(b => b.Id).ToList();

        var bookAuthors = genericRepository.Get<BookAuthor>(ba => bookIds.Contains(ba.BookId))
            .Include(ba => ba.Author)
            .ToList();

        var bookDtos = new List<BookDto>();

        foreach (var book in books)
        {
            var authors = bookAuthors
                .Where(ba => ba.BookId == book.Id)
                .Select(ba => new AuthorDto
                {
                    Id = ba.Author.Id,
                    Name = ba.Author.Name,
                    Biography = ba.Author.Biography,
                    BirthDate = ba.Author.BirthDate
                }).ToList();

            var discount = genericRepository.GetFirstOrDefault<Discount>(x => x.BookId == book.Id
            && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);
                
            bookDtos.Add(new BookDto
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
                IsAvailable = book.IsAvailable,
                // Discount = discount != null
                //     ? new DiscountDto
                //     {
                //         Id = book.Discount.Id,
                //         BookId = book.Discount.BookId,
                //         DiscountPercentage = book.Discount.DiscountPercentage,
                //         StartDate = book.Discount.StartDate,
                //         EndDate = book.Discount.EndDate,
                //         IsActive = book.Discount.IsActive,
                //         IsSaleFlag = book.Discount.IsSaleFlag
                //     }
                //     : null,
                Authors = authors
            });
        }

        return bookDtos;
    }

    public BookDto? GetById(Guid id)
    {
        var book = genericRepository.GetById<Book>(id)
                   ?? throw new NotFoundException("Book not found");

        var bookAuthors = genericRepository.Get<BookAuthor>(ba => ba.BookId == id)
            .Include(ba => ba.Author)
            .ToList();

        var authors = bookAuthors.Select(ba => new AuthorDto
        {
            Id = ba.Author.Id,
            Name = ba.Author.Name,
            Biography = ba.Author.Biography,
            BirthDate = ba.Author.BirthDate
        }).ToList();

        return new BookDto
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
            IsAvailable = book.IsAvailable,
            // Discount = book.Discount != null
            //     ? new DiscountDto
            //     {
            //         Id = book.Discount.Id,
            //         BookId = book.Discount.BookId,
            //         DiscountPercentage = book.Discount.DiscountPercentage,
            //         StartDate = book.Discount.StartDate,
            //         EndDate = book.Discount.EndDate,
            //         IsActive = book.Discount.IsActive,
            //         IsSaleFlag = book.Discount.IsSaleFlag
            //     }
            //     : null,
            Authors = authors
        };
    }

    public void Create(CreateBookDto dto)
    {
        var authors = genericRepository
            .Get<Author>(x => dto.AuthorIds.Contains(x.Id))
            .ToList();
        
        var book = new Book
        {
            Id = Guid.NewGuid(),
            PublisherId = dto.PublisherId,
            ISBN = dto.ISBN,
            Title = dto.Title,
            Description = dto.Description,
            BookFormat = dto.BookFormat,
            PublicationDate = dto.PublicationDate,
            Genre = dto.Genre,
            BasePrice = dto.BasePrice,
            PageCount = dto.PageCount,
            Language = dto.Language,
            IsAvailable = dto.IsAvailable,
            BookAuthors = authors.Select(x => new BookAuthor()
            {
                AuthorId = x.Id,
            }).ToList()
        };

        genericRepository.Insert(book);
        
    }

    public void Update(Guid id, UpdateBookDto dto)
    {
        var book = genericRepository.GetById<Book>(id)
                   ?? throw new NotFoundException("Book not found");

        book.ISBN = dto.ISBN;
        book.Title = dto.Title;
        book.Description = dto.Description;
        book.BookFormat = dto.BookFormat;
        book.PublicationDate = dto.PublicationDate;
        book.Genre = dto.Genre;
        book.BasePrice = dto.BasePrice;
        book.PageCount = dto.PageCount;
        book.Language = dto.Language;
        book.IsAvailable = dto.IsAvailable;

        genericRepository.Update(book);
    }

    public void Delete(Guid id)
    {
        var book = genericRepository.GetById<Book>(id)
                   ?? throw new NotFoundException("Book not found");

        genericRepository.Delete(book);
    }
}
