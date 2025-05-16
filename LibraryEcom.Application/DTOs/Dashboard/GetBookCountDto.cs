namespace LibraryEcom.Application.DTOs.Dashboard;

public class GetBookCountDto
{
    public int Total { get; set; }
    
    public int Available { get; set; }
    
    public int Reserved { get; set; }
}