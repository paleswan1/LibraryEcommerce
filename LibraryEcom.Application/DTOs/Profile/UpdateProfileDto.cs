using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Application.DTOs.Profile;

public class UpdateProfileDto
{
    public string Name { get; set; } = string.Empty;

    public GenderType Gender { get; set; }

    public string? Address { get; set; }
    
    public string Email { get; set; } = string.Empty;

    public string? ImageURL { get; set; }
}