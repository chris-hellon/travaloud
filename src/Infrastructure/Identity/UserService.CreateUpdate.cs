using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travaloud.Application.Common.Extensions;
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
                FullName = $"{request.FirstName} {request.LastName}",
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

    public async Task<string> CreateAsync(CreateUserRequest request, string roleName)
    {
        await using var dbContext = CreateDbContext();

        try
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = $"{request.FirstName} {request.LastName}",
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

            if (!string.IsNullOrEmpty(request.Password))
            {
                var hashedPassword = _passwordHasher.HashPassword(user, request.Password);
                user.PasswordHash = hashedPassword;
            }

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

            await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

            return user.Id;
        }
        catch (Exception ex)
        {
            return _localizer["Validation Errors Occurred."];
        }
    }

    public async Task<string> BatchCreateAsync(List<CreateUserRequest> request, string roleName)
    {
        await using var dbContext = CreateDbContext();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var usersToInsert = new List<ApplicationUser>();
            foreach (var user in request)
            {
                var username = string.IsNullOrEmpty(user.Email) ? DefaultIdType.NewGuid().ToString() : user.Email;
                var newUser = new ApplicationUser
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    UserName = username,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    Nationality = user.Nationality,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    PassportExpiryDate = user.PassportExpiryDate,
                    PassportIssueDate = user.PassportIssueDate,
                    PassportNumber = user.PassportNumber,
                    PassportIssuingCountry = user.PassportIssuingCountry,
                    VisaExpiryDate = user.VisaExpiryDate,
                    VisaIssueDate = user.VisaIssueDate,
                    ZipPostalCode = user.ZipPostalCode,
                    SignUpDate = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true,
                    NormalizedEmail = user.Email?.Normalize().ToUpper(),
                    NormalizedUserName = username.Normalize().ToUpper(),
                    LockoutEnabled = true,
                    CloudbedsGuestId = user.CloudbedsGuestId
                };

                usersToInsert.Add(newUser);
            }

            await dbContext.Users.AddRangeAsync(usersToInsert);
            await dbContext.SaveChangesAsync();

            var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName);

            if (role == null)
            {
                await transaction.RollbackAsync();
                return "No role round.";
            }

            foreach (var user in usersToInsert)
            {
                // Ensure the user exists in the database
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    var userRole = new IdentityUserRole<string> {UserId = user.Id, RoleId = role.Id};
                    dbContext.UserRoles.Add(userRole);
                }
                else
                {
                    // Handle error: User not found
                    await transaction.RollbackAsync();
                    return "User not inserted correctly.";
                }
            }

            await dbContext.SaveChangesAsync();

            // foreach (var user in usersToInsert)
            // {
            //     await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));
            // }

            await transaction.CommitAsync();

            return $"Guests Added: {usersToInsert.Count()}";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return _localizer[ex.Message];
        }
    }

    public async Task<string> BatchUpdateAsync(List<UpdateUserRequest> request)
    {
        await using var dbContext = CreateDbContext();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var usersToUpdate = new List<ApplicationUser>();
            foreach (var user in request)
            {
                var existingUser = dbContext.Users.FirstOrDefault(x => x.Id == user.Id);
                
                if (existingUser == null) continue;
                
                existingUser.CloudbedsGuestId = user.CloudbedsGuestId;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PassportNumber = user.PassportNumber;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.DateOfBirth = user.DateOfBirth;
                existingUser.FullName = $"{user.FirstName} {user.LastName}";
                existingUser.Gender = user.Gender.GenderMatch();
                existingUser.Nationality = user.Nationality;

                usersToUpdate.Add(existingUser);
            }

            dbContext.Users.UpdateRange(usersToUpdate);
            await dbContext.SaveChangesAsync();

            // foreach (var user in usersToUpdate)
            // {
            //     await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
            // }

            await transaction.CommitAsync();

            return $"Guests Updated: {usersToUpdate.Count()}";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return _localizer[ex.Message];
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
        user.FullName = $"{request.FirstName} {request.LastName}";
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