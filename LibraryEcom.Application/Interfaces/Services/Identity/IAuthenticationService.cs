using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.EmailConfirmation;
using LibraryEcom.Application.DTOs.Identity;

namespace LibraryEcom.Application.Interfaces.Services.Identity;

public interface IAuthenticationService: ITransientService
{
    Task<UserLoginResponseDto> Login(LoginDto login);
    
    Task<ResetPasswordRequestDto> ResetUserPassword(ResetUserPasswordDto resetUserPassword);
    
    Task<RegistrationResponseDto> SelfUserRegister(SelfUserRegisterDto user);

    
    Task<RegistrationResponseDto> UserRegister(UserRegisterDto user);

    void ExpireToken(string token);

    bool IsTokenExpired(string token);
}