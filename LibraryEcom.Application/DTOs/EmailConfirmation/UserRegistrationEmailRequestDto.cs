namespace LibraryEcom.Application.DTOs.EmailConfirmation;

public class UserRegistrationEmailRequestDto
{
    public string? UserId { get; set; }

    public string? EmailAddress { get; set; }
}