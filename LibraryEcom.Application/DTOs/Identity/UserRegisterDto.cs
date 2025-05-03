namespace LibraryEcom.Application.DTOs.Identity;

public class UserRegisterDto : RegisterDto
{
    public Guid RoleId { get; set; }
    
    public Guid? OrganizationId { get; set; }
}