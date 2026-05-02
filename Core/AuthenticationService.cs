using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickPOS.Data;
using QuickPOS.Models.Entities;

namespace QuickPOS.Core;

public class AuthenticationService
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly QuickPosDbContext _dbContext;
    private readonly CurrentUserProvider _currentUserProvider;

    public AppIdentityUser? CurrentUser { get; private set; }

    public AuthenticationService(
        UserManager<AppIdentityUser> userManager,
        QuickPosDbContext dbContext,
        CurrentUserProvider currentUserProvider)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return (false, "Invalid username or password");

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
            return (false, "Invalid username or password");

        CurrentUser = user;
        _currentUserProvider.Username = user.UserName;
        await LoadUserPermissions();
        return (true, "Login successful");
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        _currentUserProvider.Username = null;
        return Task.CompletedTask;
    }

    public async Task LoadUserPermissions()
    {
        if (CurrentUser != null)
        {
            var permissions = await _dbContext.UserPermissions
                .Where(p => p.UserId == CurrentUser.Id)
                .ToListAsync();

            CurrentUser.Permissions = permissions;
        }
    }

    public bool HasPermission(string permissionKey)
    {
        return CurrentUser?.Permissions.Any(p => p.PermissionKey == permissionKey) ?? false;
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        return await _dbContext.UserPermissions
            .Where(p => p.UserId == userId)
            .Select(p => p.PermissionKey)
            .ToListAsync();
    }

    public async Task<bool> UpdateUserPermissionsAsync(int userId, List<string> permissions)
    {
        try
        {
            var existing = await _dbContext.UserPermissions
                .Where(p => p.UserId == userId)
                .ToListAsync();

            _dbContext.UserPermissions.RemoveRange(existing);

            var newPermissions = permissions.Select(p => new UserPermission
            {
                UserId = userId,
                PermissionKey = p
            }).ToList();

            await _dbContext.UserPermissions.AddRangeAsync(newPermissions);
            await _dbContext.SaveChangesAsync();

            // Reload if it's the current user
            if (CurrentUser?.Id == userId)
            {
                await LoadUserPermissions();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
