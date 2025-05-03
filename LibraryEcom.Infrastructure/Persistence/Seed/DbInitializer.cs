using LibraryEcom.Domain.Entities.Identity;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Data;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Domain.Common.Enum;
using LibraryEcom.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryEcom.Infrastructure.Persistence.Seed;

public class DbInitializer(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IApplicationDbContext dbContext,
    IWebHostEnvironment webHostEnvironment) : IDbInitializer
{
    public async Task InitializeIdentityData(CancellationToken cancellationToken = default)
    {
        var user = await InitializeAdmin(cancellationToken);

        await InitializeRoles();

        if (!await userManager.IsInRoleAsync(user, Constants.Roles.Admin))
        {
            await userManager.AddToRoleAsync(user, Constants.Roles.Admin);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InitializeRoles()
    {
        var roles = new List<string>
        {
            Constants.Roles.Admin,
            Constants.Roles.Staff,
            Constants.Roles.User
        };

        for (var i = 0; i < roles.Count; i++)
        {
            var role = roles[i];
            if (!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new Role
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                };

                var result = await roleManager.CreateAsync(newRole);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToArray();
                    throw new BadRequestException($"Failed to create role: {role}", errors);
                }
            }
        }
    }

    private async Task<User> InitializeAdmin(CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(Constants.Admin.Identifier);

        if (user is null)
        {
            var adminUser = new User
            {
                Id = Guid.Parse(Constants.Admin.Identifier),
                Name = Constants.Admin.Development.Name,
                Email = Constants.Admin.Development.EmailAddress,
                UserName = Constants.Admin.Development.EmailAddress,
                NormalizedEmail = Constants.Admin.Development.EmailAddress.ToUpper(),
                NormalizedUserName = Constants.Admin.Development.EmailAddress.ToUpper(),
                PhoneNumber = "+977-9800000000",
                Gender = GenderType.Male,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, Constants.Admin.Development.DecryptedPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                throw new BadRequestException("Failed to create admin user", errors);
            }

            return adminUser;
        }

        // Update values if already exists
        user.Name = Constants.Admin.Development.Name;
        user.Email = Constants.Admin.Development.EmailAddress;
        user.NormalizedEmail = Constants.Admin.Development.EmailAddress.ToUpper();
        user.NormalizedUserName = Constants.Admin.Development.EmailAddress.ToUpper();
        user.PasswordHash = Constants.Admin.Development.DecryptedPassword.HashPassword();
        user.EmailConfirmed = true;
        user.PhoneNumberConfirmed = true;

        await userManager.UpdateAsync(user);

        return user;
    }
}
