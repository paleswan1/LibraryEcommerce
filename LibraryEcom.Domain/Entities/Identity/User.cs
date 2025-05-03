using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Enum;

namespace LibraryEcom.Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    
    public GenderType Gender { get; set; } 
    
    public string? Address { get; set; }
    
    public string? ImageURL { get; set; }
    
    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; }
    
    public void ActivateDeactivateUser()
    {
        IsActive = !IsActive;
        EmailConfirmed = !EmailConfirmed;
    }
    
}