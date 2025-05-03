using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Identity;

namespace LibraryEcom.Application.Interfaces.Services.Identity;

public interface IProfileService : ITransientService
{
    UserDetail GetUserProfile();

    RolesDto GetUserRole();

    void UpdateUserProfile(ProfileRequestDto profileDetails);

    void UpdateProfileImage(ProfileImageRequestDto profileImage);

    Task ChangePassword(ChangePasswordRequestDto changePasswordDto);

    void DeleteUserProfile();
}