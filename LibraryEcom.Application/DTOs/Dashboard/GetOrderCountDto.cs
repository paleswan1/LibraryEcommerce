namespace LibraryEcom.Application.DTOs.Dashboard;

public class GetOrderCountDto
{
    public int Total { get; set; }
    
    public int Pending { get; set; }
    
    public int Completed { get; set; }
}