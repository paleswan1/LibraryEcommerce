namespace LibraryEcom.Application.DTOs.Dashboard;

public class GetRecentOrderDetailsDto
{
    public string Id { get; set; }
    
    public string Member { get; set; }
    
    public string Book { get; set; }
    
    public string Status { get; set; }
    
    public DateOnly Date { get; set; }
}