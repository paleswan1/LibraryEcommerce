    using System.Net;
    using LibraryCom.Controllers.Base;
    using LibraryEcom.Application.Common.Response;
    using LibraryEcom.Application.DTOs.Book;
    using LibraryEcom.Application.Interfaces.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace LibraryCom.Controllers;

    [Route("api/book")]
    [ApiController]
    [Authorize]
    public class BookController(IBookService bookService) : BaseController<BookController>
    {
        [HttpGet]
        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("list")]
        public IActionResult GetList(string? search = null)
        {
            var books = bookService.GetAll(search);

            return Ok(new ResponseDto<List<BookDto>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book list retrieved successfully.",
                Result = books
            });
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromForm] CreateBookDto bookDto)
        {
            bookService.Create(bookDto);

            return Ok(new ResponseDto<bool>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book created successfully.",
                Result = true
            });
        }

        [AllowAnonymous]
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromForm] UpdateBookDto bookDto)
        {
            bookService.Update(id, bookDto);

            return Ok(new ResponseDto<bool>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book updated successfully.",
                Result = true
            });
        }

        [AllowAnonymous]
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
