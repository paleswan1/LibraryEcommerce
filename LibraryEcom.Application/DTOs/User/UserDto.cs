using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Application.DTOs.User;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public GenderType Gender { get; set; } 
    
    public string? Address { get; set; }
    
    public string? ImageURL { get; set; }
    
    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; }
    
    public string EmailAddress { get; set; }
    
    
}