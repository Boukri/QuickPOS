using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ardalis.Specification;
using Microsoft.AspNetCore.Identity;
using QuickPOS.Core;
using QuickPOS.Data;
using QuickPOS.Data.Specifications;
using QuickPOS.Models.Entities;

namespace QuickPOS.Features.Users;

public partial class UsersViewModel : ViewModelBase
{
    private readonly IRepositoryBase<AppIdentityUser> _userRepo;
    private readonly UserManager<AppIdentityUser> _userManager;

    public ObservableCollection<UserRowViewModel> Users { get; } = [];

    [ObservableProperty] private bool _isFormVisible;
    [ObservableProperty] private string _formFullName = string.Empty;
    [ObservableProperty] private string _formEmail = string.Empty;
    [ObservableProperty] private string _formRole = "Staff";
    [ObservableProperty] private string _formPassword = string.Empty;

    private int? _editingUserId;

    public string[] Roles { get; } = ["Admin", "Manager", "Staff"];

    public UsersViewModel(IRepositoryBase<AppIdentityUser> userRepo, UserManager<AppIdentityUser> userManager)
    {
        _userRepo = userRepo;
        _userManager = userManager;
        LoadUsersCommand.ExecuteAsync(null);
    }

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

    [RelayCommand]
    private void ShowAddForm()
    {
        _editingUserId = null;
        FormFullName = string.Empty;
        FormEmail = string.Empty;
        FormRole = "Staff";
        FormPassword = string.Empty;
        IsFormVisible = true;
    }

    [RelayCommand]
    private void EditUser(UserRowViewModel row)
    {
        _editingUserId = row.Id;
        FormFullName = row.FullName;
        FormEmail = row.Email;
        FormRole = row.Role;
        FormPassword = string.Empty;
        IsFormVisible = true;
    }

    [RelayCommand]
    private async Task SaveUser()
    {
        if (_editingUserId.HasValue)
        {
            var user = await _userRepo.GetByIdAsync(_editingUserId.Value);
            if (user is not null)
            {
                user.FullName = FormFullName;
                user.Email = FormEmail;
                user.UserName = FormEmail;
                await _userManager.UpdateAsync(user);
                // Update role
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, FormRole);
                // Update password only if provided
                if (!string.IsNullOrWhiteSpace(FormPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, FormPassword);
                }
            }
        }
        else
        {
            var user = new AppIdentityUser
            {
                FullName = FormFullName,
                Email = FormEmail,
                UserName = FormEmail
            };
            await _userManager.CreateAsync(user, FormPassword);
            await _userManager.AddToRoleAsync(user, FormRole);
        }
        IsFormVisible = false;
        await LoadUsers();
    }

    [RelayCommand]
    private async Task DeleteUser(UserRowViewModel row)
    {
        var user = await _userRepo.GetByIdAsync(row.Id);
        if (user is not null)
        {
            await _userManager.DeleteAsync(user);
            await LoadUsers();
        }
    }

    [RelayCommand]
    private void CancelForm() => IsFormVisible = false;

    public static string GetRoleDescription(string role) => role switch
    {
        "Admin" => "Full system access: manage users, settings, reports, and all operations.",
        "Manager" => "Can manage inventory, process sales, view reports, and manage staff schedules.",
        "Staff" => "Can process sales, view own performance, and manage assigned tasks.",
        _ => "No description available."
    };
}

public class UserRowViewModel
{
    public int Id { get; }
    public string FullName { get; }
    public string Email { get; }
    public string Role { get; }
    public string Initials { get; }
    public string RoleDescription { get; }

    public UserRowViewModel(AppIdentityUser u, string role)
    {
        Id = u.Id;
        FullName = u.FullName;
        Email = u.Email ?? string.Empty;
        Role = role;
        var parts = u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Initials = parts.Length >= 2 ? $"{parts[0][0]}{parts[1][0]}" : (u.FullName.Length >= 2 ? u.FullName[..2] : u.FullName);
        RoleDescription = UsersViewModel.GetRoleDescription(role);
    }
}
