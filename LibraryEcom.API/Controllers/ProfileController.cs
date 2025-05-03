using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Profile;
using LibraryEcom.Application.DTOs.User;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/profile")]
public class ProfileController(IProfileService profileService) : BaseController<ProfileController>
{
    [HttpGet]
    public IActionResult GetProfile()
    {
        var profile = profileService.GetUserProfile();

        return Ok(new ResponseDto<GetProfileDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User profile retrieved successfully.",
            Result = profile
        });
    }

    [HttpPut]
    public IActionResult UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        profileService.UpdateUserProfile(dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Profile updated successfully.",
            Result = true
        });
    }

    [HttpPut("image")]
    public IActionResult UpdateProfileImage([FromForm] UpdateUserImageDto dto)
    {
        profileService.UpdateProfileImage(dto);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Profile image updated successfully.",
            Result = true
        });
    }

    [HttpPut("change-password")]
    public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
    {
        return BadRequest(new ResponseDto<string>
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = "Password change must be handled by the identity service.",
            Result = "Not implemented"
        });
    }
}