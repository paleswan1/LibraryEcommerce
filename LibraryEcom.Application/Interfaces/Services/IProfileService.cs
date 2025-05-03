using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Profile;
using LibraryEcom.Application.DTOs.User;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IProfileService: ITransientService
{
    GetProfileDto GetUserProfile();

    void UpdateUserProfile(UpdateProfileDto profile);
    
    void UpdateProfileImage(UpdateUserImageDto profileImage);
    
    Task ChangePassword(ChangePasswordDto changePassword);
}