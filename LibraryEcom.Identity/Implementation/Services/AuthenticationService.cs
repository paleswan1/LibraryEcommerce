using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.EmailConfirmation;
using LibraryEcom.Application.DTOs.Identity;
using LibraryEcom.Application.Interfaces.Services.Identity;
using LibraryEcom.Application.Settings;
using LibraryEcom.Domain.Common;
using LibraryEcom.Domain.Entities.Identity;
using LibraryEcom.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Helper.Implementation.Manager;
using LibraryEcom.Infrastructure.Implementation.Services;
using Microsoft.AspNetCore.Authentication;
using IAuthenticationService = LibraryEcom.Application.Interfaces.Services.Identity.IAuthenticationService;

namespace LibraryEcom.Identity.Implementation.Services;

public class AuthenticationService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<JwtSettings> jwtSettings,
    ICurrentUserService userService,
    TokenManager tokenManager,
    IGenericRepository genericRepository,
    IFileService fileService,
    IEmailService emailService) : IAuthenticationService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IFileService _fileService = fileService; 


    private const string UsersImagesFilePath = Constants.FilePath.UsersImagesFilePath;

    private static bool IsValidProvider(string provider) =>
        provider is Constants.Provider.Api or Constants.Provider.Wasm;
    
    private static string GenerateRandomPassword(int length = 12)
    {
        if (length < 2)
            throw new ArgumentException(
                "Password length must be at least 2 to include both alphanumeric and non-alphanumeric characters.");

        const string numericChars = "0123456789";
        const string nonAlphanumericChars = "!@#$%&";
        const string alphanumericChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        var random = new Random();
        var remainingChars = new char[length - 2];
        var numericChar = numericChars[random.Next(nonAlphanumericChars.Length)];
        var nonAlphaChar = nonAlphanumericChars[random.Next(nonAlphanumericChars.Length)];

        for (var i = 0; i < remainingChars.Length; i++)
        {
            remainingChars[i] = alphanumericChars[random.Next(alphanumericChars.Length)];
        }

        var combinedPassword = nonAlphaChar.ToString() + numericChar + new string(remainingChars);

        return new string(combinedPassword.OrderBy(_ => random.Next()).ToArray());
    }

    public async Task<UserLoginResponseDto> Login(LoginDto login)
    {
        var user = await userManager.FindByEmailAsync(login.Email)
            ?? throw new NotFoundException("The following user has not been registered to our system.");

        if (!user.IsActive)
            throw new BadRequestException("You can not log in to the system.",
                ["The following user is not active, please contact the administrator"]);
        
        var isPasswordValid = await userManager.CheckPasswordAsync(user, login.Password);

        if (!isPasswordValid)
            throw new NotFoundException("Invalid password, please try again.");

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var issuer = _jwtSettings.Issuer;

        var audience = _jwtSettings.Audience;

        var accessTokenExpirationInMinutes = Convert.ToInt32(_jwtSettings.AccessTokenExpirationInMinutes);
        
        var userRoles = await userManager.GetRolesAsync(user);
        
        var roleName = userRoles.FirstOrDefault();
        
        var role = await roleManager.FindByNameAsync(roleName!);

        var AuthClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, role!.Name!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var symmetricSigningKey = new SymmetricSecurityKey(key);

        var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

        var expirationTime = ExtensionMethod.GetUtcDate().AddMinutes(accessTokenExpirationInMinutes);

        var accessToken = new JwtSecurityToken(
            issuer,
            audience,
            claims: AuthClaims,
            signingCredentials: signingCredentials,
            expires: expirationTime
        );    
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(accessToken);
        
        var userDetails = new UserDetail()
        {
           Id = user.Id,
           Email = user.Email,
           Name = user.Name,
           IsActive = user.IsActive,
           Address = user.Address,
           RegisteredDate = user.RegisteredDate,
           RoleName = userRoles.FirstOrDefault(),
           
        };

        var result = new UserLoginResponseDto()
        {
            Token = jwt,
            UserDetails = userDetails
        };

        return result;
    }

    public Task<ResetPasswordRequestDto> ResetUserPassword(ResetUserPasswordDto resetUserPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<RegistrationResponseDto> SelfUserRegister(SelfUserRegisterDto user)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
            throw new NotFoundException("A user with this email already exists. Please try a different one.");

        if (user.Password != user.ConfirmPassword)
            throw new BadRequestException("Password and Confirm Password do not match.",
                new[] { "Ensure both passwords are identical." });

        string? userImageUrl = null;
        if (user.ImageUrl != null)
        {
            var uploadedFileName = fileService.UploadDocument(user.ImageUrl, UsersImagesFilePath, user.Email);
            userImageUrl = $"{UsersImagesFilePath}/{uploadedFileName}";
        }

        var newUser = new User
        {
            Name = user.Name,
            UserName = user.Email,
            NormalizedUserName = user.Email.ToUpper(),
            Email = user.Email,
            NormalizedEmail = user.Email.ToUpper(),
            Gender = user.Gender,
            Address = user.Address,
            ImageURL = userImageUrl,
            IsActive = true,
            EmailConfirmed = true,
            RegisteredDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(newUser, user.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            throw new BadRequestException("User registration failed.", errors);
        }

        // Assign default role (e.g., "User")
        var defaultRole = "User"; // Ensure this exists in your DB
        if (await roleManager.RoleExistsAsync(defaultRole))
        {
            await userManager.AddToRoleAsync(newUser, defaultRole);
        }

        return new RegistrationResponseDto
        {
            UserId = newUser.Id.ToString()
        };
    }




    public async Task<RegistrationResponseDto> UserRegister(UserRegisterDto user)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email);
        
        if(existingUser != null)
            throw new NotFoundException(
                "An existing user with the following email address already exists in our system, please try again with a new email address.");
        //
        // var userImageUrl = user.ImageUrl != null
        //     ? .UploadDocument(user.ImageUrl, UserSecretsIdAttribute
        //     : null;

        if (user.Password != user.ConfirmPassword)
        {
            var exception = new[]
            {
                "The password do not match with confirm password.",
            };

            throw new BadRequestException("The following client admin could not be added", exception);
        }

        var userModel = new User()
        {
            Name = user.Name,
            UserName = user.Email,
            NormalizedUserName = user.Email.ToUpper(),
            Email = user.Email,
            NormalizedEmail = user.Email.ToUpper(),
            Address = user.Address,
            IsActive = true,
            EmailConfirmed = true,
            RegisteredDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(userModel, user.Password);

        if (!result.Succeeded)
        {
            var role = await roleManager.FindByIdAsync(user.RoleId.ToString());
            if (role != null)
            {
                await userManager.AddToRoleAsync(userModel, role.Name);
            }

        }
        else
        {
            var errors = result.Errors.Select(x => $"{x.Description}");

            throw new BadRequestException("The following user couldn't be created.", errors.ToArray());
        }

        var userDetail = await userManager.FindByEmailAsync(user.Email);

        var response = new RegistrationResponseDto()
        {
            UserId = user?.RoleId.ToString(),
        };
        
        return response;

    }

    public void ExpireToken(string token)
    {     
        tokenManager.BlackList.Add(token);
    }

    public bool IsTokenExpired(string token)
    {
        return tokenManager.BlackList.Contains(token);
    }
}