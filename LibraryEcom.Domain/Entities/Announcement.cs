using LibraryEcom.Domain.Common.Base;

namespace LibraryEcom.Domain.Entities;

public class Announcement: BaseEntity<Guid>
{
    public string Message { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}