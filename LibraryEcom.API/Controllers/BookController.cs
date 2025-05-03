using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Book;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/book")]
public class BookController(IBookService bookService) : BaseController<BookController>
{
    [HttpGet]
    public IActionResult Get(int pageNumber, int pageSize, string? search = null)
    {
        var books = bookService.GetAll(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<BookDto>(books, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Books retrieved successfully.",
            Result = books
        });
    }

    [HttpGet("all")]
    public IActionResult GetAll(string? search = null)
    {
        var books = bookService.GetAll(search);

        return Ok(new ResponseDto<List<BookDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "All books retrieved successfully.",
            Result = books
        });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var book = bookService.GetById(id);

        return Ok(new ResponseDto<BookDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book retrieved successfully.",
            Result = book
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateBookDto dto)
    {
        bookService.Create(dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book created successfully.",
            Result = true
        });
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateBookDto dto)
    {
        bookService.Update(id, dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book updated successfully.",
            Result = true
        });
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        bookService.Delete(id);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book deleted successfully.",
            Result = true
        });
    }
}
