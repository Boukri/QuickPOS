using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickPOS.Core;

namespace QuickPOS.Features.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthenticationService _authService;
    private readonly UserSettingsService _settings;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _rememberMe;

    public event EventHandler? LoginSuccessful;

    public LoginViewModel(AuthenticationService authService, UserSettingsService settings)
    {
        _authService = authService;
        _settings = settings;

        RememberMe = settings.Settings.RememberMe;
        if (RememberMe && !string.IsNullOrEmpty(settings.Settings.AutoLoginUsername))
            Username = settings.Settings.AutoLoginUsername;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both username and password";
            return;
        }

        IsLoading = true;

        try
        {
            var (success, message) = await _authService.LoginAsync(Username, Password);

            if (success)
            {
                _settings.Settings.RememberMe = RememberMe;
                _settings.Settings.AutoLoginUsername = RememberMe ? Username : string.Empty;
                _settings.Save();

                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = message;
                Password = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
