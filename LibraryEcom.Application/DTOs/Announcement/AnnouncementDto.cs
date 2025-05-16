namespace LibraryEcom.Application.DTOs.Announcement;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    
    public string Message { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}