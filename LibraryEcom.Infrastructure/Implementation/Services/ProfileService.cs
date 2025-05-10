using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.Profile;
using LibraryEcom.Application.DTOs.User;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class ProfileService(
    IGenericRepository genericRepository,
    ICurrentUserService currentUserService,
    IFileService fileService) : IProfileService
{
    private const string UsersImagesFilePath = "Uploads/Users"; 

    public GetProfileDto GetUserProfile()
    {
        var userId = currentUserService.GetUserId;

        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("User not found.");

        return new GetProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            EmailAddress = user.Email,
            Gender = user.Gender,
            Address = user.Address,
            IsActive = user.IsActive,
            RegisteredDate = user.RegisteredDate,
            ImageURL = user.ImageURL,
        };
    }

    public void UpdateUserProfile(UpdateProfileDto profile)
    {
        throw new NotImplementedException();
    }

    public void UpdateProfile(UpdateProfileDto dto)
    {
        var userId = currentUserService.GetUserId;

        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("User not found.");

        user.Name = dto.Name;
        user.Gender = dto.Gender;
        user.Address = dto.Address;

        genericRepository.Update(user);
    }

    public void UpdateProfileImage(UpdateUserImageDto profileImage)
    {
        var userId = currentUserService.GetUserId;

        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("User not found.");

        if (!string.IsNullOrEmpty(user.ImageURL))
        {
            var fullPath = Path.Combine(UsersImagesFilePath, user.ImageURL);
            fileService.DeleteFile(fullPath);
        }

        user.ImageURL = fileService.UploadDocument(profileImage.Image, UsersImagesFilePath);
        genericRepository.Update(user);
    }

    public Task ChangePassword(ChangePasswordDto dto)
    {
        throw new NotImplementedException("Change password is handled via authentication service.");
    }
}
