using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Application.DTOs.Identity;

public class ProfileRequestDto
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }
    
    public string Address { get; set; }
    
    public GenderType Gender { get; set; }
}
