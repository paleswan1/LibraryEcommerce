using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Application.DTOs.Review;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Domain.Common.Enum;
using LibraryEcom.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class BookService(IGenericRepository genericRepository) : IBookService
{
    public List<BookDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null,
        bool? isActive = null)
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
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var reviews = genericRepository.Get<Review>(r => bookIds.Contains(r.BookId)).ToList();

            var authors = bookAuthors
                .Where(ba => ba.BookId == book.Id)
                .Select(ba => new AuthorDto
                {
                    Id = ba.Author.Id,
                    Name = ba.Author.Name,
                    Biography = ba.Author.Biography,
                    BirthDate = ba.Author.BirthDate
                }).ToList();

            var validDiscount = genericRepository.GetFirstOrDefault<Discount>(
                x => x.BookId == book.Id && x.StartDate <= today && x.EndDate >= today
            );

            var bookReviews = reviews
                .Where(r => r.BookId == book.Id)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    ReviewDate = r.ReviewDate
                }).ToList();

            var allDiscounts = genericRepository.Get<Discount>(d => d.BookId == book.Id).ToList()
                .Select(d => new DiscountDto
                {
                    Id = d.Id,
                    DiscountPercentage = d.DiscountPercentage,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    BookId = d.BookId,
                    IsSaleFlag = d.IsSaleFlag
                }).ToList();

            DiscountDto discountDto = null;

            if (validDiscount != null)
            {
                discountDto = new DiscountDto
                {
                    Id = validDiscount.Id,
                    BookId = validDiscount.BookId,
                    DiscountPercentage = validDiscount.DiscountPercentage,
                    StartDate = validDiscount.StartDate,
                    EndDate = validDiscount.EndDate,
                    IsSaleFlag = validDiscount.IsSaleFlag,
                };
            }

            bookDtos.Add(new BookDto
            {
                Id = book.Id,
                PublisherName = book.PublisherName,
                ISBN = book.ISBN,
                Title = book.Title,
                Description = book.Description,
                BookFormat = book.BookFormat,
                PublicationDate = book.PublicationDate,
                CoverImage = book.CoverImage,
                Genre = book.Genre,
                BasePrice = book.BasePrice,
                PageCount = book.PageCount,
                Language = book.Language,
                IsAvailable = book.IsAvailable,
                Discount = allDiscounts,
                Authors = authors,
                Reviews = bookReviews,
                ValidatedDiscount = discountDto
            });
        }

        return bookDtos;
    }

    public List<BookDto> GetAll(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? search = null,
        bool? isActive = null,
        string? sortBy = null,
        bool isDescending = false)
    {
        var query = genericRepository.Get<Book>()
            .Where(x =>
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()) ||
                 x.Description.ToLower().Contains(search.ToLower())) &&
                (isActive == null || x.IsAvailable == isActive)
            );

        query = sortBy?.ToLower() switch
        {
            "title" => isDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "price" => isDescending ? query.OrderByDescending(b => b.BasePrice) : query.OrderBy(b => b.BasePrice),
            "date" => isDescending
                ? query.OrderByDescending(b => b.PublicationDate)
                : query.OrderBy(b => b.PublicationDate),
            _ => query.OrderBy(b => b.Title)
        };

        rowCount = query.Count();

        var books = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var bookIds = books.Select(b => b.Id).ToList();

        var bookAuthors = genericRepository.Get<BookAuthor>(ba => bookIds.Contains(ba.BookId))
            .Include(ba => ba.Author)
            .ToList();

        var allDiscounts = genericRepository.Get<Discount>(d => bookIds.Contains(d.BookId)).ToList();


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

            var today = DateOnly.FromDateTime(DateTime.UtcNow);


            var activeDiscounts = allDiscounts
                .Where(d => d.BookId == book.Id && d.StartDate <= today && d.EndDate >= today)
                .Select(d => new DiscountDto
                {
                    Id = d.Id,
                    BookId = d.BookId,
                    DiscountPercentage = d.DiscountPercentage,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    IsSaleFlag = d.IsSaleFlag,
                }).ToList();

            bookDtos.Add(new BookDto
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
                CoverImage = book.CoverImage,
                Language = book.Language,
                IsAvailable = book.IsAvailable,
                Discount = activeDiscounts,
                Authors = authors
            });
        }

        return bookDtos;
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

        var allDiscounts = genericRepository.Get<Discount>(d => bookIds.Contains(d.BookId)).ToList();

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
                                                                              && x.StartDate <=
                                                                              DateOnly.FromDateTime(DateTime.Now) &&
                                                                              x.EndDate >=
                                                                              DateOnly.FromDateTime(DateTime.Now));

            var today = DateOnly.FromDateTime(DateTime.UtcNow);


            var activeDiscounts = allDiscounts
                .Where(d => d.BookId == book.Id && d.StartDate <= today && d.EndDate >= today)
                .Select(d => new DiscountDto
                {
                    Id = d.Id,
                    BookId = d.BookId,
                    DiscountPercentage = d.DiscountPercentage,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    IsSaleFlag = d.IsSaleFlag,
                }).ToList();

            bookDtos.Add(new BookDto
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
                CoverImage = book.CoverImage,
                PageCount = book.PageCount,
                Language = book.Language,
                IsAvailable = book.IsAvailable,
                Discount = activeDiscounts,
                Authors = authors
            });
        }

        return bookDtos;
    }

    public BookDto? GetById(Guid id)
    {
        var book = genericRepository.GetById<Book>(id)
                   ?? throw new NotFoundException("Book not found.");

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
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        
        var validDiscount = genericRepository.GetFirstOrDefault<Discount>(
            x => x.BookId == book.Id && x.StartDate <= today && x.EndDate >= today
        );
        
        var allDiscounts = genericRepository.Get<Discount>(d => d.BookId == book.Id).ToList()
            .Select(d => new DiscountDto
            {
                Id = d.Id,
                DiscountPercentage = d.DiscountPercentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                BookId = d.BookId,
                IsSaleFlag = d.IsSaleFlag
            }).ToList();

        DiscountDto discountDto = null;

        if (validDiscount != null)
        {
            discountDto = new DiscountDto
            {
                Id = validDiscount.Id,
                BookId = validDiscount.BookId,
                DiscountPercentage = validDiscount.DiscountPercentage,
                StartDate = validDiscount.StartDate,
                EndDate = validDiscount.EndDate,
                IsSaleFlag = validDiscount.IsSaleFlag,
            };
        }
        
        return new BookDto
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
            CoverImage = book.CoverImage,
            Language = book.Language,
            IsAvailable = book.IsAvailable,
            Discount = allDiscounts,
            Authors = authors,
            ValidatedDiscount = discountDto,
        };
    }

    public void Create(CreateBookDto dto)
    {
        var authors = genericRepository
            .Get<Author>(x => dto.AuthorIds.Contains(x.Id))
            .ToList();

        var coverImage = dto.CoverImage != null ? UploadImage(dto.CoverImage) : null;

        var bookId = Guid.NewGuid();

        var book = new Book
        {
            Id = bookId,
            PublisherName = dto.PublisherName,
            ISBN = dto.ISBN,
            Title = dto.Title,
            Description = dto.Description,
            BookFormat = dto.BookFormat,
            PublicationDate = dto.PublicationDate,
            Genre = dto.Genre,
            CoverImage = coverImage,
            BasePrice = dto.BasePrice,
            PageCount = dto.PageCount,
            Language = dto.Language,
            IsAvailable = dto.IsAvailable,
            BookAuthors = authors.Select(x => new BookAuthor
            {
                BookId = bookId,
                AuthorId = x.Id
            }).ToList()
        };

        try
        {
            genericRepository.Insert(book);
        }
        catch (Exception ex)
        {
            throw new Exception("Error saving book" + ex.InnerException?.Message ?? ex.Message);
        }
    }

    public string UploadImage(IFormFile file)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);

        file.CopyTo(stream);

        return fileName;
    }

    public void DeleteImage(string fileName)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }


    public void Update(Guid id, UpdateBookDto dto)
    {
        var book = genericRepository.GetById<Book>(id)
                   ?? throw new NotFoundException("Book not found");

        // Handle cover image update
        if (dto.CoverImage != null)
        {
            // Delete old image if exists
            if (!string.IsNullOrWhiteSpace(book.CoverImage))
            {
                DeleteImage(book.CoverImage);
            }

            book.CoverImage = UploadImage(dto.CoverImage);
        }

        book.PublisherName = dto.PublisherName;
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

        if (dto.AuthorIds != null && dto.AuthorIds.Any())
        {
            var authors = genericRepository.Get<Author>(x => dto.AuthorIds.Contains(x.Id)).ToList();

            var existingAuthors = genericRepository.Get<BookAuthor>(x => x.BookId == book.Id).ToList();
            foreach (var ba in existingAuthors)
            {
                genericRepository.Delete(ba);
            }

            // Add new author links
            book.BookAuthors = authors.Select(a => new BookAuthor
            {
                BookId = book.Id,
                AuthorId = a.Id
            }).ToList();
        }

        try
        {
            genericRepository.Update(book);
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating book: " + ex.InnerException?.Message ?? ex.Message);
        }
    }


    public void Delete(Guid id)
    {
        var book = genericRepository.GetById<Book>(id)
                   ?? throw new NotFoundException("Book not found");

        genericRepository.Delete(book);
    }

    public PagedBookResponseDto GetBooksByGenre(string? genre, int pageNumber, int pageSize, string? search)
    {
        throw new NotImplementedException();
    }

    public List<BookCategoryDto> GetBooksByCategory(BookCategory category)
    {
        throw new NotImplementedException();
    }
}