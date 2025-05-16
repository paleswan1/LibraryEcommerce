using Microsoft.AspNetCore.Http;
using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Application.DTOs.User;

public class UpdateUserRequestDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public GenderType Gender { get; set; }

    public string? Address { get; set; }
    
    public IFormFile? Image { get; set; }
}