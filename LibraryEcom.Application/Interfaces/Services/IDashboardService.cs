using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Dashboard;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IDashboardService: ITransientService
{
    Task<List<GetRecentOrderDto>> GetMonthlyOrdersAsync(int numberOfMonths);
    
    Task<GetLibraryDashboardOverviewDto> GetLibraryDashboardOverviewAsync(int period);


}