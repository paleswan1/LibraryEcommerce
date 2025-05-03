using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IAuthorService: ITransientService
{
    List<AuthorDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<AuthorDto> GetAll(string? search = null);
    
    AuthorDto? GetById(Guid id);

    void Create(CreateAuthorDto dto);

    void Update(Guid id, UpdateAuthorDto dto);

    void Delete(Guid id);
}