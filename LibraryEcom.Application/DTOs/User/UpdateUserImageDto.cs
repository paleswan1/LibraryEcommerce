using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Application.DTOs.User;

public class UpdateUserImageDto
{
    public required IFormFile Image { get; set; }

}