namespace LibraryEcom.Application.DTOs.Announcement;

public class CreateAnnouncementDto
{
    public string Message { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}