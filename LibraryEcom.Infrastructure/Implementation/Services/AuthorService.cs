using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class AuthorService(IGenericRepository genericRepository):IAuthorService
{
    public List<AuthorDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var authors = genericRepository.GetPagedResult<Author>(pageNumber, pageSize, out rowCount, 
            x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList(); 
        
        var authorDtos = new List<AuthorDto>();

        foreach (var author in authors)
        {
            authorDtos.Add(new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BirthDate = author.BirthDate,
            });
        }
        
        return authorDtos;
    }

    public List<AuthorDto> GetAll(string? search = null)
    {
        var authors = genericRepository
            .Get<Author>(x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())))
            .ToList();
        
        var authorDtos = new List<AuthorDto>();

        foreach (var author in authors)
        {
            authorDtos.Add(new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BirthDate = author.BirthDate,
            });
        }
        return authorDtos;
    }

    public AuthorDto? GetById(Guid id)
    {
        var author = genericRepository.GetById<Author>(id)
            ?? throw new NullReferenceException("The following entity was not found");

        var result = new AuthorDto()
        {
            Id = author.Id,
            Name = author.Name,
            Biography = author.Biography,
            BirthDate = author.BirthDate,
        };
        
        return result;
    }

    public void Create(CreateAuthorDto dto)
    {
        var existing = genericRepository.GetFirstOrDefault<Author>(x => 
            x.Name.ToLower() == dto.Name.ToLower()
            && x.Biography == dto.Biography &&
            x.BirthDate == dto.BirthDate);

        if (existing != null)
        {
            throw new NotFoundException("An identical announcement already exists.");
        }

        var model = new Author
        {
            Name = dto.Name,
            Biography = dto.Biography,
            BirthDate = dto.BirthDate,
        };
        
        genericRepository.Insert(model);
    }

    public void Update(Guid id, UpdateAuthorDto dto)
    {
        var model = genericRepository.GetById<Author>(id)
                    ?? throw new NotFoundException("The following author with specified Id was not found.");
        
        model.Name = dto.Name;
        model.Biography = dto.Biography;
        model.BirthDate = dto.BirthDate;
        
        genericRepository.Update(model);    }

    public void Delete(Guid id)
    {
        var author = genericRepository.GetById<Author>(id)
                           ?? throw new NotFoundException("The following author with specified Id was not found.");

        genericRepository.Delete(author);    }
}