using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using QuickPOS.Core;
using QuickPOS.Features.DailyClose;
using QuickPOS.Features.Dashboard;
using QuickPOS.Features.Inventory;
using QuickPOS.Features.Products;
using QuickPOS.Features.Selling;
using QuickPOS.Features.Users;
using QuickPOS.Models.Entities;

namespace QuickPOS;

public partial class MainViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly AuthenticationService _authService;

    public NavigationService Navigation => _navigationService;
    public LocalizationService Loc => LocalizationService.Instance;

    [ObservableProperty] private string _currentPageTitle = string.Empty;
    [ObservableProperty] private int _currentNavIndex;
    [ObservableProperty] private string _currentUserName = string.Empty;
    [ObservableProperty] private string _currentUserInitials = string.Empty;
    [ObservableProperty] private bool _canAccessSelling = true;
    [ObservableProperty] private bool _canAccessProducts = true;
    [ObservableProperty] private bool _canAccessDashboard = true;
    [ObservableProperty] private bool _canAccessUsers = true;
    [ObservableProperty] private bool _canAccessDailyClose = true;
    [ObservableProperty] private bool _canAccessInventory = true;

    public bool IsLoggingOut { get; private set; }

    public MainViewModel(NavigationService navigationService, AuthenticationService authService)
    {
        _navigationService = navigationService;
        _authService = authService;

        LoadUserInfo();
        LoadPermissions();
        NavigateToFirstAvailable();
    }

    private void LoadUserInfo()
    {
        var user = _authService.CurrentUser;
        if (user != null)
        {
            CurrentUserName = user.FullName;
            CurrentUserInitials = GetInitials(user.FullName);
        }
    }

    private void LoadPermissions()
    {
        CanAccessSelling    = _authService.HasPermission(Permissions.NavSelling);
        CanAccessProducts   = _authService.HasPermission(Permissions.NavProducts);
        CanAccessDashboard  = _authService.HasPermission(Permissions.NavDashboard);
        CanAccessUsers      = _authService.HasPermission(Permissions.NavUsers);
        CanAccessDailyClose = _authService.HasPermission(Permissions.NavDailyClose);
        CanAccessInventory  = _authService.HasPermission(Permissions.NavInventory);
    }

    private void NavigateToFirstAvailable()
    {
        if (CanAccessSelling)    { NavigateToSelling();    return; }
        if (CanAccessProducts)   { NavigateToProducts();   return; }
        if (CanAccessInventory)  { NavigateToInventory();  return; }
        if (CanAccessDashboard)  { NavigateToDashboard();  return; }
        if (CanAccessUsers)      { NavigateToUsers();      return; }
        if (CanAccessDailyClose) { NavigateToDailyClose(); return; }
    }

    private string GetInitials(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "U";
        if (parts.Length == 1) return parts[0][..1].ToUpper();
        return (parts[0][..1] + parts[^1][..1]).ToUpper();
    }

    [RelayCommand]
    private void NavigateToSelling()
    {
        if (!CanAccessSelling) return;
        CurrentPageTitle = Loc.PageTitleSelling;
        CurrentNavIndex = 0;
        _navigationService.NavigateTo<SellingViewModel>();
    }

    [RelayCommand]
    private void NavigateToProducts()
    {
        if (!CanAccessProducts) return;
        CurrentPageTitle = Loc.PageTitleProducts;
        CurrentNavIndex = 1;
        _navigationService.NavigateTo<ProductsViewModel>();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        if (!CanAccessDashboard) return;
        CurrentPageTitle = Loc.PageTitleDashboard;
        CurrentNavIndex = 2;
        _navigationService.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void NavigateToUsers()
    {
        if (!CanAccessUsers) return;
        CurrentPageTitle = Loc.PageTitleUsers;
        CurrentNavIndex = 3;
        _navigationService.NavigateTo<UsersViewModel>();
    }

    [RelayCommand]
    private void NavigateToDailyClose()
    {
        if (!CanAccessDailyClose) return;
        CurrentPageTitle = Loc.PageTitleDailyClose;
        CurrentNavIndex = 4;
        _navigationService.NavigateTo<DailyCloseViewModel>();
    }

    [RelayCommand]
    private void NavigateToInventory()
    {
        if (!CanAccessInventory) return;
        CurrentPageTitle = Loc.PageTitleInventory;
        CurrentNavIndex  = 5;
        _navigationService.NavigateTo<InventoryBatchViewModel>();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        IsLoggingOut = true;
        await _authService.LogoutAsync();
        Application.Current.MainWindow?.Close();
        ((App)Application.Current).ShowLoginLoop();
    }

    [RelayCommand]
    private void SetFrench() => Loc.Language = AppLanguage.French;

    [RelayCommand]
    private void SetArabic() => Loc.Language = AppLanguage.Arabic;
}
