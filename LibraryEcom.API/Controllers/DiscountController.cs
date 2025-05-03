using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Discount;
using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/discount")]
[ApiController]
public class DiscountController(IDiscountService discountService) : BaseController<DiscountController>
{
    [HttpGet]
    public IActionResult GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        var discounts = discountService.GetAll(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<DiscountDto>(discounts, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Discounts retrieved successfully.",
            Result = discounts
        });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var discount = discountService.GetById(id);

        return Ok(new ResponseDto<DiscountDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Discount retrieved successfully.",
            Result = discount
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateDiscountDto dto)
    {
        discountService.Create(dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Discount created successfully.",
            Result = true
        });
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateDiscountDto dto)
    {
        discountService.Update(id, dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Discount updated successfully.",
            Result = true
        });
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        discountService.Delete(id);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Discount deleted successfully.",
            Result = true
        });
    }

    [HttpGet("active")]
    public IActionResult GetActiveDiscounts()
    {
        var discounts = discountService.GetActiveDiscounts();

        return Ok(new ResponseDto<List<DiscountDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Active discounts retrieved successfully.",
            Result = discounts
        });
    }

    [HttpPut("toggle-sale/{id:guid}")]
    public IActionResult ToggleSaleFlag(Guid id, [FromQuery] bool isSale)
    {
        discountService.ToggleSaleFlag(id, isSale);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = $"Discount sale flag set to {isSale} successfully.",
            Result = true
        });
    }
}
