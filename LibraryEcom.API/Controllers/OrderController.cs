using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Order;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/order")]
public class OrderController : BaseController<OrderController>
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult GetAll(int pageNumber, int pageSize, string? search = null)
    {
        var orders = _orderService.GetAll(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<OrderDto>(orders, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Orders retrieved successfully.",
            Result = orders
        });
    }

    [HttpGet("all")]
    public IActionResult GetAll(string? search = null)
    {
        var orders = _orderService.GetAll(search);

        return Ok(new ResponseDto<List<OrderDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "All orders retrieved successfully.",
            Result = orders
        });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var order = _orderService.GetById(id);

        return Ok(new ResponseDto<OrderDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order retrieved successfully.",
            Result = order
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderDto dto)
    {
        _orderService.Create(dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order created successfully.",
            Result = true
        });
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateOrderDto dto)
    {
        _orderService.Update(id, dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order updated successfully.",
            Result = true
        });
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _orderService.Delete(id);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order deleted successfully.",
            Result = true
        });
    }

    [HttpPost("fulfill-by-claim")]
    [Authorize(Roles = "Staff")]
    public IActionResult FulfillOrder([FromBody] string claimCode)
    {
        _orderService.FulfillOrderByClaimCode(claimCode);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order successfully fulfilled.",
            Result = true
        });
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrderFromCart()
    {
        var orderId = await _orderService.PlaceOrder();

        return Ok(new ResponseDto<Guid>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order placed successfully from cart.",
            Result = orderId
        });
    }

    
    [HttpPut("cancel/{orderId:guid}")]
    public IActionResult CancelOrder(Guid orderId)
    {
        _orderService.CancelOrder(orderId);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Order cancelled successfully.",
            Result = true
        });
    }
}
