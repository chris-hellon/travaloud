using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.Identity.Users.Password;
using Travaloud.Domain.Common;
using Travaloud.Domain.Identity;

namespace Travaloud.Infrastructure.Identity;

internal partial class UserService
{
    public async Task ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        await using var dbContext = CreateDbContext();
        var user = await GetUserById(dbContext, userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var hashedPassword = _passwordHasher.HashPassword(user, model.NewPassword);
        user.PasswordHash = hashedPassword;
        
        dbContext.Users.Update(user);

        await dbContext.SaveChangesAsync();
        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    public async Task<string> CreateAsync(CreateUserRequest request, string origin, string roleName)
    {
        await using var dbContext = CreateDbContext();

        try
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                Nationality = request.Nationality,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                PassportExpiryDate = request.PassportExpiryDate,
                PassportIssueDate = request.PassportIssueDate,
                PassportNumber = request.PassportNumber,
                PassportIssuingCountry = request.PassportIssuingCountry,
                VisaExpiryDate = request.VisaExpiryDate,
                VisaIssueDate = request.VisaIssueDate,
                ZipPostalCode = request.ZipPostalCode,
                SignUpDate = DateTime.Now,
                IsActive = true,
                EmailConfirmed = true,
                NormalizedEmail = request.Email.Normalize().ToUpper(),
                NormalizedUserName = request.Email.Normalize().ToUpper(),
                LockoutEnabled = true
            };

            var hashedPassword = _passwordHasher.HashPassword(user, request.Password);
            user.PasswordHash = hashedPassword;

            await dbContext.Users.AddAsync(user);

            await dbContext.SaveChangesAsync();

            var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
            if (role != null)
            {
                await dbContext.UserRoles.AddAsync(new IdentityUserRole<string>()
                {
                    UserId = user.Id,
                    RoleId = role!.Id
                });   
            }

            await dbContext.SaveChangesAsync();

            var messages = new List<string> {string.Format(_localizer["User {0} Registered."], user.UserName)};
            
            await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

            return string.Join(Environment.NewLine, messages);
        }
        catch (Exception)
        {
            return _localizer["Validation Errors Occurred."];
        }
    }

    public async Task UpdateAsync(UpdateUserRequest request, string userId)
    {
        await using var dbContext = CreateDbContext();
        var user = await GetUserById(dbContext, userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var currentImage = user.ImageUrl ?? string.Empty;
        if (request.Image != null || request.DeleteCurrentImage)
        {
            user.ImageUrl = await _fileStorage.UploadAsync<ApplicationUser>(request.Image, FileType.Image);
            if (request.DeleteCurrentImage && !string.IsNullOrEmpty(currentImage))
            {
                var root = Directory.GetCurrentDirectory();
                await _fileStorage.Remove(Path.Combine(root, currentImage));
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.Address = request.Address;
        user.City = request.City;
        user.ZipPostalCode = request.ZipPostalCode;
        user.Gender = request.Gender;
        user.Nationality = request.Nationality;
        user.DateOfBirth = request.DateOfBirth;
        user.PassportNumber = request.PassportNumber;
        user.PassportIssueDate = request.PassportIssueDate;
        user.PassportExpiryDate = request.PassportExpiryDate;
        user.PassportIssuingCountry = request.PassportIssuingCountry;
        user.VisaIssueDate = request.VisaIssueDate;
        user.VisaExpiryDate = request.VisaExpiryDate;

        try
        {
            dbContext.Users.Update(user);

            await dbContext.SaveChangesAsync();
            await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
        }
        catch (Exception)
        {
            throw new InternalServerException(_localizer["Update profile failed"]);
        }
    }
}