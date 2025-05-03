using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Author;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/author")]
public class AuthorController(IAuthorService authorService): BaseController<AuthorController>
{
    [HttpGet]
    public IActionResult Get(int pageNumber, int pageSize, string? search)
    {
        var authors = authorService.GetAll(pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<AuthorDto>(authors, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Authors successfully retrieved",
            Result = authors
        });
    }
    
    [HttpGet("{authorId:guid}")]
    public IActionResult GetAuthor(Guid authorId)
    {
        var author = authorService.GetById(authorId);

        return Ok(new ResponseDto<AuthorDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Author successfully retrieved.",
            Result = author
        });
    }
    
    [HttpPost]
    public IActionResult InsertAuthor(CreateAuthorDto author)
    {
        authorService.Create(author);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Author successfully added.",
            Result = true
        });
    }
    
    [HttpPut("{authorId:guid}")]
    public IActionResult UpdateAuthor(UpdateAuthorDto author, Guid authorId)
    {
        authorService.Update(authorId, author);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Author successfully updated.",
            Result = true
        });
    }

    [HttpPatch("{authorId:guid}")]
    public IActionResult DeleteAuthor(Guid authorId)
    {
        authorService.Delete(authorId);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Author successfully deleted.",
            Result = true
        });
    }
    
    
}