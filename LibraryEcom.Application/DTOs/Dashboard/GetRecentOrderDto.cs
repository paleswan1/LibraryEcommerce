namespace LibraryEcom.Application.DTOs.Dashboard;

public class GetRecentOrderDto
{
    public int Year { get; set; }    
    
    public int Month { get; set; }
    
    public int TotalOrders { get; set; }
}