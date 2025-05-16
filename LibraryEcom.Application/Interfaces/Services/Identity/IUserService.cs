using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Identity;
using LibraryEcom.Application.DTOs.User;

namespace LibraryEcom.Application.Interfaces.Services.Identity;

public interface IUserService : ITransientService
{
    UserDetail GetUserProfileById(Guid userId);

    List<UserResponseDto> GetUsersByRole(int pageNumber, int pageSize, out int rowCount, bool? isActive = null, string? search = null, Guid? roleId = default);

    List<UserResponseDto> GetUsersByRole(bool? isActive = null, string? search = null, Guid? roleId = default);

    List<UserResponseDto> GetUsersForClientOrganization(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<UserResponseDto> GetUsersForClientOrganization(string? search = null, bool? isActive = null);
    
    void UpdateUserDetails(UpdateUserRequestDto user);
    
    void ActivateDeactivateUsers(Guid userId);
    
    void DeleteUser(Guid userId);
}
