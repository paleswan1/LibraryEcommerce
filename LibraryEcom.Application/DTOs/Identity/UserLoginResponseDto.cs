namespace LibraryEcom.Application.DTOs.Identity;

public class UserLoginResponseDto
{
    public string Token { get; set; }

    public UserDetail UserDetails { get; set; }
}

public class UserDetail
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public Guid RoleId { get; set; }

    public string RoleName { get; set; }

    public string? ImageUrl { get; set; }

    public string Gender { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string? Address { get; set; }

   public string Username { get; set; }
   
   public string LastName { get; set; }
   
   public string FirstName { get; set; }
    
   public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
}
