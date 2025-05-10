using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Domain.Common.Enum;
using LibraryEcom.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IBookService: ITransientService
{
    List<BookDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);
    
    List<BookDto> GetAll(string? search = null);
    
    BookDto? GetById(Guid id);

    void Create(CreateBookDto dto);
    
    string UploadImage(IFormFile file);
    
    void DeleteImage(string fileName);

    void Update(Guid id, UpdateBookDto dto);

    void Delete(Guid id);
    
    PagedBookResponseDto GetBooksByGenre(string? genre, int pageNumber, int pageSize, string? search);
    
    List<BookCategoryDto> GetBooksByCategory(BookCategory category);


}