using System.Windows;
using System.Windows.Controls;
using Material.Icons;
using Material.Icons.WPF;
using QuickPOS.Core;

namespace QuickPOS.Features.Auth;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Set initial checked state based on current language (avoids binding converter at init time)
        if (LocalizationService.Instance.IsArabic)
            LangArBtn.IsChecked = true;
        else
            LangFrBtn.IsChecked = true;

        viewModel.LoginSuccessful += (_, _) =>
        {
            DialogResult = true;
            Close();
        };
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            vm.Password = pb.Password;
    }

    private void EyeToggle_Checked(object sender, RoutedEventArgs e)
    {
        PasswordText.Visibility = Visibility.Visible;
        PasswordBox.Visibility = Visibility.Collapsed;
        EyeIcon.Kind = MaterialIconKind.EyeOffOutline;
        EyeToggle.ToolTip = LocalizationService.Instance.HidePassword;
    }

    private void EyeToggle_Unchecked(object sender, RoutedEventArgs e)
    {
        PasswordBox.Visibility = Visibility.Visible;
        PasswordText.Visibility = Visibility.Collapsed;
        PasswordBox.Password = PasswordText.Text;
        EyeIcon.Kind = MaterialIconKind.EyeOutline;
        EyeToggle.ToolTip = LocalizationService.Instance.ShowPassword;
    }

    private void LangFr_Checked(object sender, RoutedEventArgs e)
        => LocalizationService.Instance.Language = AppLanguage.French;

    private void LangAr_Checked(object sender, RoutedEventArgs e)
        => LocalizationService.Instance.Language = AppLanguage.Arabic;

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
