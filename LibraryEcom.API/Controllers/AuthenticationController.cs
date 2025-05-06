using System.Net;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Identity;
using LibraryEcom.Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryCom.Controllers.Base;

namespace LibraryCom.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController(IAuthenticationService authenticationService) 
    : BaseController<AuthenticationController>
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
    {
        var result = await authenticationService.Login(loginRequest);

        return Ok(new ResponseDto<UserLoginResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Successfully authenticated.",
            Result = result,
        });
    }

    [AllowAnonymous]
    [HttpPost("register/self")]
    public async Task<IActionResult> SelfRegister([FromForm] SelfUserRegisterDto registerRequest)
    {
        var result = await authenticationService.SelfUserRegister(registerRequest);

        return Ok(new ResponseDto<RegistrationResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User successfully registered.",
            Result = result,
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto registerRequest)
    {
        var result = await authenticationService.UserRegister(registerRequest);

        return Ok(new ResponseDto<RegistrationResponseDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User successfully registered by admin.",
            Result = result,
        });
    }

}
