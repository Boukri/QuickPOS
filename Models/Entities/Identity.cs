using Microsoft.AspNetCore.Identity;

namespace QuickPOS.Models.Entities;

public class AppIdentityUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;

    // Navigation property for permissions
    public virtual ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
}

public class AppIdentityRole : IdentityRole<int>
{
    public string? Description { get; set; }
}

public class UserPermission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PermissionKey { get; set; } = string.Empty; // e.g., "Nav.Selling", "Nav.Products", etc.

    public virtual AppIdentityUser User { get; set; } = null!;
}

public static class Permissions
{
    public const string NavSelling = "Nav.Selling";
    public const string NavProducts = "Nav.Products";
    public const string NavDashboard = "Nav.Dashboard";
    public const string NavUsers = "Nav.Users";
    public const string NavDailyClose = "Nav.DailyClose";

    public static readonly string[] AllPermissions = 
    {
        NavSelling,
        NavProducts,
        NavDashboard,
        NavUsers,
        NavDailyClose
    };

    public static string GetDisplayName(string permission) => permission switch
    {
        NavSelling => "POS Terminal",
        NavProducts => "Products & Services",
        NavDashboard => "Financial Dashboard",
        NavUsers => "Users & Roles",
        NavDailyClose => "Daily Close",
        _ => permission
    };
}
