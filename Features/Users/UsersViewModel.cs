using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ardalis.Specification; 
using QuickPOS.Core; 
using QuickPOS.Data.Specifications;
using QuickPOS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace QuickPOS.Features.Users;

public partial class UsersViewModel : ViewModelBase
{
    private readonly IRepositoryBase<AppIdentityUser> _userRepo;
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly RoleManager<AppIdentityRole> _roleManager;
    private readonly AuthenticationService _authService;

    public ObservableCollection<UserRowViewModel> Users { get; } = [];

    // ── Form state ───────────────────────────────────────────────────────────
    [ObservableProperty] private bool   _isFormVisible;
    [ObservableProperty] private string _formTitle      = string.Empty;
    [ObservableProperty] private string _formFullName   = string.Empty;
    [ObservableProperty] private string _formEmail      = string.Empty;
    [ObservableProperty] private string _formPassword   = string.Empty;
    [ObservableProperty] private string _formStatus     = "Active";
    [ObservableProperty] private ObservableCollection<PermissionCheckItem> _formPermissions = [];

    private int? _editingUserId;

    // ── Delete confirmation state ────────────────────────────────────────────
    [ObservableProperty] private bool _isDeleteConfirmVisible;
    private UserRowViewModel? _pendingDeleteUser;

    public string[] Statuses { get; } = ["Active", "Inactive"];

    // ── Constructor ──────────────────────────────────────────────────────────
    public UsersViewModel(
        IRepositoryBase<AppIdentityUser> userRepo,
        UserManager<AppIdentityUser>     userManager,
        RoleManager<AppIdentityRole> roleManager,
        AuthenticationService            authService)
    {
        _userRepo    = userRepo;
        _userManager = userManager;
        _authService = authService;
        _roleManager = roleManager;
        LoadUsers().ConfigureAwait(false);
    }

    // ── Load ─────────────────────────────────────────────────────────────────
    [RelayCommand]
    private async Task LoadUsers()
    {
        IsBusy = true;
        try
        {
            var allUsers = await _userRepo.ListAsync(new AllIdentityUsersSpec());
            Users.Clear();
            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                Users.Add(new UserRowViewModel(u, roles.FirstOrDefault() ?? "Staff"));
            }
        }
        finally { IsBusy = false; }
    }

    // ── Open form ────────────────────────────────────────────────────────────
    [RelayCommand]
    private void ShowAddForm()
    {
        _editingUserId   = null;
        FormTitle        = LocalizationService.Instance.UsersAddTitle;
        FormFullName     = string.Empty;
        FormEmail        = string.Empty;
        FormPassword     = string.Empty;
        FormStatus       = "Active";
        FormPermissions  = BuildFormPermissions([]);
        IsFormVisible    = true;
    }

    [RelayCommand]
    private async Task EditUser(UserRowViewModel row)
    {
        _editingUserId  = row.Id;
        FormTitle       = LocalizationService.Instance.UsersFormTitle;
        FormFullName    = row.FullName;
        FormEmail       = row.Email;
        FormPassword    = string.Empty;
        FormStatus      = row.Status;
        var existingKeys = await _authService.GetUserPermissionsAsync(row.Id);
        FormPermissions = BuildFormPermissions(existingKeys);
        IsFormVisible   = true;
    }

    // ── Save ─────────────────────────────────────────────────────────────────
    [RelayCommand]
    private async Task SaveUser()
    {
        var selectedKeys = FormPermissions
            .Where(p => p.IsChecked)
            .Select(p => p.Key)
            .ToList();

        if (_editingUserId.HasValue)
        {
            var user = await _userRepo.GetByIdAsync(_editingUserId.Value);
            if (user is not null)
            {
                user.FullName  = FormFullName;
                user.Email     = FormEmail;
                user.UserName  = FormEmail;
                user.LockoutEnd = FormStatus == "Inactive" ? DateTimeOffset.MaxValue : null;

                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrWhiteSpace(FormPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, FormPassword);
                }

                await _authService.UpdateUserPermissionsAsync(user.Id, selectedKeys);
            }
        }
        else
        {
            var user = new AppIdentityUser
            {
                FullName   = FormFullName,
                Email      = FormEmail,
                UserName   = FormEmail,
                LockoutEnd = FormStatus == "Inactive" ? DateTimeOffset.MaxValue : null
            };

            var defaultRole = new AppIdentityRole
            {
                Name = "Staff",
                NormalizedName = "STAFF",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Description = GetRoleDescription("Staff")
            };
            var result = await _userManager.CreateAsync(user, FormPassword);
            if (result.Succeeded)
            {
                var staffRole = await _roleManager.FindByNameAsync(defaultRole.Name);

                if (staffRole is null)
                {
                    await _roleManager.CreateAsync(defaultRole);
                }
                await _userManager.AddToRoleAsync(user, defaultRole.Name);
                await _authService.UpdateUserPermissionsAsync(user.Id, selectedKeys);
            }
        }

        IsFormVisible = false;
        await LoadUsers();
    }

    // ── Delete (admins are protected) ────────────────────────────────────────
    [RelayCommand]
    private void RequestDeleteUser(UserRowViewModel row)
    {
        if (!row.CanDelete) return;
        _pendingDeleteUser = row;
        IsDeleteConfirmVisible = true;
    }

    [RelayCommand]
    private async Task ConfirmDeleteUser()
    {
        if (_pendingDeleteUser is not null)
        {
            var user = await _userRepo.GetByIdAsync(_pendingDeleteUser.Id);
            if (user is not null)
            {
                // Double-check role at delete time (defensive)
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    await _userManager.DeleteAsync(user);
            }
            await LoadUsers();
        }
        _pendingDeleteUser = null;
        IsDeleteConfirmVisible = false;
    }

    [RelayCommand]
    private void CancelDeleteUser()
    {
        _pendingDeleteUser = null;
        IsDeleteConfirmVisible = false;
    }

    [RelayCommand]
    private void CancelForm() => IsFormVisible = false;

    // ── Helpers ──────────────────────────────────────────────────────────────
    private static ObservableCollection<PermissionCheckItem> BuildFormPermissions(
        IEnumerable<string> heldKeys)
    {
        var loc  = LocalizationService.Instance;
        var held = heldKeys.ToHashSet();

        return
        [
            new(Permissions.NavSelling,    loc.NavPosTerminal, held.Contains(Permissions.NavSelling)),
            new(Permissions.NavProducts,   loc.NavProducts,    held.Contains(Permissions.NavProducts)),
            new(Permissions.NavInventory,  loc.NavInventory,   held.Contains(Permissions.NavInventory)),
            new(Permissions.NavDashboard,  loc.NavDashboard,   held.Contains(Permissions.NavDashboard)),
            new(Permissions.NavDailyClose, loc.NavDailyClose,  held.Contains(Permissions.NavDailyClose)),
            new(Permissions.NavUsers,      loc.NavUsers,       held.Contains(Permissions.NavUsers)),
        ];
    }

    public static string GetRoleDescription(string role) => role switch
    {
        "Admin"   => "Full system access: manage users, settings, reports, and all operations.",
        "Manager" => "Can manage inventory, process sales, view reports, and manage staff schedules.",
        _         => "Can process sales, view own performance, and manage assigned tasks."
    };
}

// ── Row model ─────────────────────────────────────────────────────────────────

public class UserRowViewModel
{
    public int    Id              { get; }
    public string FullName        { get; }
    public string Email           { get; }
    public string Role            { get; }
    public string Status          { get; }
    public string Initials        { get; }
    public string RoleDescription { get; }

    /// <summary>False for Admin-role users — hides the Delete button.</summary>
    public bool CanDelete { get; }

    public UserRowViewModel(AppIdentityUser u, string role)
    {
        Id     = u.Id;
        FullName = u.FullName;
        Email  = u.Email ?? string.Empty;
        Role   = role;
        Status = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow
                 ? "Inactive" : "Active";

        var parts = u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Initials = parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}"
            : (u.FullName.Length >= 2 ? u.FullName[..2] : u.FullName);

        RoleDescription = UsersViewModel.GetRoleDescription(role);
        CanDelete       = role != "Admin";
    }
}

// ── Permission checkbox item ───────────────────────────────────────────────────

public partial class PermissionCheckItem : ObservableObject
{
    public string Key         { get; }
    public string DisplayName { get; }

    [ObservableProperty] private bool _isChecked;

    public PermissionCheckItem(string key, string displayName, bool isChecked = false)
    {
        Key         = key;
        DisplayName = displayName;
        IsChecked   = isChecked;
    }
}
