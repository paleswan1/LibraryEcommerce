using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IBookService: ITransientService
{
    List<BookDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<BookDto> GetAll(string? search = null);
    
    BookDto? GetById(Guid id);

    void Create(CreateBookDto dto);

    void Update(Guid id, UpdateBookDto dto);

    void Delete(Guid id);
}