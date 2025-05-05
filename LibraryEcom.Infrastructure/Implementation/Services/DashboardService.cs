using LibraryEcom.Application.DTOs.Dashboard;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class DashboardService : IDashboardService
{
    private readonly IGenericRepository _genericRepository;

    public DashboardService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<GetRecentOrderDto>> GetMonthlyOrdersAsync(int numberOfMonths)
    {
        var now = DateTime.UtcNow;
        var fromDate = now.AddMonths(-numberOfMonths + 1);

        var groupedOrders = await _genericRepository
            .Get<Order>(order => order.CreatedAt >= fromDate)
            .GroupBy(o => new { Year = o.CreatedAt.Year, Month = o.CreatedAt.Month })
            .Select(g => new GetRecentOrderDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalOrders = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return groupedOrders;
    }
}