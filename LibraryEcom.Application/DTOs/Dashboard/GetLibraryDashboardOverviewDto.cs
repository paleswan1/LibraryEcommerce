namespace LibraryEcom.Application.DTOs.Dashboard;

public class GetLibraryDashboardOverviewDto
{
    // 📚 Books
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public double? BookGrowthPercent { get; set; }

    // 👥 Members
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int NewMembers { get; set; }
    public double? MemberGrowthPercent { get; set; }

    // 🛒 Orders
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public double? OrderGrowthPercent { get; set; }

    // 📝 Reviews
    public int TotalReviews { get; set; }
    public double? ReviewGrowthPercent { get; set; }

    public List<GetRecentOrderDetailsDto> RecentOrders { get; set; } = [];

    // 📈 Monthly stats
    public List<GetRecentOrderDto> MonthlyOrders { get; set; } = [];
}