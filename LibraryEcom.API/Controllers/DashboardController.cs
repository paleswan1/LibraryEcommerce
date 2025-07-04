using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Dashboard;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController(IDashboardService dashboardService) : BaseController<DashboardController>
{
    [HttpGet("monthly-orders")]
    public async Task<IActionResult> GetMonthlyOrders([FromQuery] int months = 6)
    {
        var result = await dashboardService.GetMonthlyOrdersAsync(months);

        return Ok(new ResponseDto<List<GetRecentOrderDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Monthly orders retrieved successfully.",
            Result = result
        });
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview([FromQuery] int period)
    {
        var result = await dashboardService.GetLibraryDashboardOverviewAsync(period);

        return Ok(new ResponseDto<GetLibraryDashboardOverviewDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Library dashboard overview retrieved successfully.",
            Result = result
        });
    }
}