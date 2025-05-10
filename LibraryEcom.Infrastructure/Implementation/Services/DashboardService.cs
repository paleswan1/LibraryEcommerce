using LibraryEcom.Application.DTOs.Dashboard;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class DashboardService(IGenericRepository genericRepository) : IDashboardService
{
    public async Task<List<GetRecentOrderDto>> GetMonthlyOrdersAsync(int numberOfMonths)
    {
        var now = DateTime.UtcNow;
        var fromDate = now.AddMonths(-numberOfMonths + 1);

        var groupedOrders = await genericRepository
            .Get<Order>(order => order.CreatedAt >= fromDate)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
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

    public async Task<GetLibraryDashboardOverviewDto> GetLibraryDashboardOverviewAsync(int period)
    {
        var (currentStart, currentEnd, prevStart, prevEnd) = GetPeriodRange(period);

        var totalBooks = await genericRepository.Get<Book>().CountAsync();
        var availableBooks = await genericRepository.Get<Book>().Where(b => b.IsAvailable).CountAsync();
        var currentBooks = await genericRepository.Get<Book>()
            .Where(b => b.CreatedAt >= currentStart && b.CreatedAt <= currentEnd).CountAsync();
        var prevBooks = await genericRepository.Get<Book>()
            .Where(b => b.CreatedAt >= prevStart && b.CreatedAt <= prevEnd).CountAsync();
        var bookGrowth = CalculateGrowth(currentBooks, prevBooks);

        var totalMembers = await genericRepository.Get<User>().CountAsync();
        var activeMembers = await genericRepository.Get<User>().Where(m => m.IsActive).CountAsync();


        var totalOrders = await genericRepository.Get<Order>().CountAsync();
        var currentOrders = await genericRepository.Get<Order>()
            .Where(o => o.CreatedAt >= currentStart && o.CreatedAt <= currentEnd).CountAsync();
        var prevOrders = await genericRepository.Get<Order>()
            .Where(o => o.CreatedAt >= prevStart && o.CreatedAt <= prevEnd).CountAsync();
        var orderGrowth = CalculateGrowth(currentOrders, prevOrders);

        var totalReviews = await genericRepository.Get<Review>().CountAsync();
        var currentReviews = await genericRepository.Get<Review>()
            .Where(r => r.CreatedAt >= currentStart && r.CreatedAt <= currentEnd).CountAsync();
        var prevReviews = await genericRepository.Get<Review>()
            .Where(r => r.CreatedAt >= prevStart && r.CreatedAt <= prevEnd).CountAsync();
        var reviewGrowth = CalculateGrowth(currentReviews, prevReviews);

        var recentOrdersQuery = genericRepository.Get<Order>()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5);

        var recentOrders = await recentOrdersQuery
            .Select(o => new GetRecentOrderDetailsDto
            {
                Id = o.Id.ToString(),
                Member = o.User.Name,
                Book = o.OrderItems.Select(oi => oi.Book.Title).FirstOrDefault() ?? "N/A",
                Status = o.Status,
                Date = DateOnly.FromDateTime(o.CreatedAt)
            })
            .ToListAsync();

        var now = DateTime.UtcNow;
        var fromDate = now.AddMonths(-4);
        var monthlyOrders = await genericRepository
            .Get<Order>(o => o.CreatedAt >= fromDate)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new GetRecentOrderDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalOrders = g.Count()
            })
            .OrderBy(o => o.Year)
            .ThenBy(o => o.Month)
            .ToListAsync();

        return new GetLibraryDashboardOverviewDto
        {
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            BookGrowthPercent = bookGrowth,
            TotalMembers = totalMembers,
            ActiveMembers = activeMembers,
            TotalOrders = totalOrders,
            OrderGrowthPercent = orderGrowth,
            TotalReviews = totalReviews,
            ReviewGrowthPercent = reviewGrowth,
            RecentOrders = recentOrders,
            MonthlyOrders = monthlyOrders
        };
    }

    private static (DateTime currentStart, DateTime currentEnd, DateTime prevStart, DateTime prevEnd)
        GetPeriodRange(int period)
    {
        var today = DateTime.UtcNow;
        return period switch
        {
            Constants.TimePeriod.Weekly => (
                today.StartOfWeek(),
                today.StartOfWeek().AddDays(6),
                today.StartOfWeek().AddDays(-7),
                today.StartOfWeek().AddDays(-1)
            ),
            Constants.TimePeriod.Monthly => (
                new DateTime(today.Year, today.Month, 1),
                new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1),
                new DateTime(today.Year, today.Month, 1).AddMonths(-1),
                new DateTime(today.Year, today.Month, 1).AddDays(-1)
            ),
            Constants.TimePeriod.Yearly => (
                new DateTime(today.Year, 1, 1),
                new DateTime(today.Year, 12, 31),
                new DateTime(today.Year - 1, 1, 1),
                new DateTime(today.Year - 1, 12, 31)
            ),
            _ => (DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MinValue)
        };
    }

    private static double? CalculateGrowth(int current, int previous)
    {
        return previous == 0 ? null : Math.Round(((double)(current - previous) / previous) * 100, 2);
    }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}