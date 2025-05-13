namespace LibraryEcom.Application.DTOs.Dashboard;

public class GetWeeklyOrderDto
{
    public int Year { get; set; }
    
    public int Week { get; set; } 
    
    public int TotalOrders { get; set; }
}