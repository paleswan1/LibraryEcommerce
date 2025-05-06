using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Whitelist;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/whitelist")]
public class WhitelistController(IWhitelistService whitelistService) : BaseController<WhitelistController>
{
    [HttpGet]
    public IActionResult GetUserWishlist([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var wishlist = whitelistService.GetUserWishlist(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<WhiteListDto>(wishlist, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Wishlist successfully retrieved.",
            Result = wishlist
        });
    }

    [HttpGet("all")]
    public IActionResult GetAllUserWishlist()
    {
        var wishlist = whitelistService.GetUserWishlist();

        return Ok(new ResponseDto<List<WhiteListDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Wishlist successfully retrieved.",
            Result = wishlist
        });
    }

    [HttpPost]
    public IActionResult AddToWishlist([FromForm] CreateWhiteListDto dto)
    {
        whitelistService.AddToWishlist(dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book successfully added to wishlist.",
            Result = true
        });
    }

    [HttpDelete("{id:guid}")]
    public IActionResult RemoveFromWishlist(Guid id)
    {
        whitelistService.RemoveFromWishlist(id);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book successfully removed from wishlist.",
            Result = true
        });
    }

    [HttpGet("check/{bookId:guid}")]
    public IActionResult IsBookWishlisted(Guid bookId)
    {
        var isWishlisted = whitelistService.IsBookWishlisted(bookId);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = isWishlisted ? "Book is in wishlist." : "Book is not in wishlist.",
            Result = isWishlisted
        });
    }
}
