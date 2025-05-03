using LibraryEcom.Application.DTOs.Cart;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryEcom.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpGet("paged")]
    public ActionResult<List<CartDto>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var carts = cartService.GetAll(pageNumber, pageSize, out int rowCount);
        Response.Headers.Add("X-Total-Count", rowCount.ToString());
        return Ok(carts);
    }

    [HttpGet]
    public ActionResult<List<CartDto>> GetAll()
    {
        var carts = cartService.GetAll();
        return Ok(carts);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<CartDto> GetById(Guid id)
    {
        var cart = cartService.GetById(id);
        return cart == null ? NotFound() : Ok(cart);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateCartDto dto)
    {
        cartService.Create(dto);
        return Ok(new { message = "Book added to cart successfully." });
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        cartService.Delete(id);
        return Ok(new { message = "Cart item removed successfully." });
    }

    [HttpPut("increase/{bookId:guid}")]
    public IActionResult IncreaseQuantity(Guid bookId)
    {
        cartService.IncreaseQuantity(bookId);
        return Ok(new { message = "Book quantity increased." });
    }

    [HttpPut("decrease/{bookId:guid}")]
    public IActionResult DecreaseQuantity(Guid bookId)
    {
        cartService.DecreaseQuantity(bookId);
        return Ok(new { message = "Book quantity decreased." });
    }

    [HttpPost("place-order")]
    public IActionResult PlaceOrder()
    {
        var orderId = cartService.PlaceOrderFromCart();
        return Ok(new { message = "Order placed successfully.", orderId });
    }

    [HttpDelete("cancel-order/{orderId:guid}")]
    public IActionResult CancelOrder(Guid orderId)
    {
        cartService.CancelOrder(orderId);
        return Ok(new { message = "Order cancelled successfully." });
    }
}
