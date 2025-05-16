using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Cart;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcom.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartService cartService) : BaseController<CartController>
{
    [HttpGet("paged")]
    public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var carts = cartService.GetAll(pageNumber, pageSize, out int rowCount);

        return Ok(new CollectionDto<CartDto>(carts, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Carts retrieved successfully.",
            Result = carts
        });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var carts = cartService.GetAll();
        return Ok(new ResponseDto<List<CartDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "All carts retrieved successfully.",
            Result = carts
        });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var cart = cartService.GetById(id);
        if (cart == null)
        {
            return NotFound(new ResponseDto<CartDto>
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = "Cart item not found.",
                Result = null
            });
        }

        return Ok(new ResponseDto<CartDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Cart item retrieved successfully.",
            Result = cart
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateCartDto dto)
    {
        cartService.Create(dto);
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book added to cart successfully.",
            Result = true
        });
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        cartService.Delete(id);
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Cart item removed successfully.",
            Result = true
        });
    }

    [HttpPut("increase/{bookId:guid}")]
    public IActionResult IncreaseQuantity(Guid bookId)
    {
        cartService.IncreaseQuantity(bookId);
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book quantity increased.",
            Result = true
        });
    }

    [HttpPut("decrease/{bookId:guid}")]
    public IActionResult DecreaseQuantity(Guid bookId)
    {
        cartService.DecreaseQuantity(bookId);
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Book quantity decreased.",
            Result = true
        });
    }

    [HttpPost("place-order")]
    public IActionResult PlaceOrder()
    {
        var orderId = cartService.PlaceOrderFromCart();
        return Ok(new ResponseDto<Guid>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order placed successfully.",
            Result = orderId
        });
    }

    [HttpDelete("cancel-order/{orderId:guid}")]
    public IActionResult CancelOrder(Guid orderId)
    {
        cartService.CancelOrder(orderId);
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order cancelled successfully.",
            Result = true
        });
    }

    [HttpDelete("clear-all")]
    public IActionResult ClearCart()
    {
        cartService.ClearCart();
        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Cart cleared successfully.",
            Result = true
        });
    }

}
