using LibraryEcom.Application.Common.Attributes;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Application.DTOs.Identity;

public class ProfileImageRequestDto
{
    [FileExamination(5 * 1024 * 1024)]
    public IFormFile ImageUrl { get; set; }
}