using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using QuickPOS.Core;

namespace QuickPOS.Features.Selling;

public partial class SellingView : UserControl
{
    private UserSettingsService? _settings;

    public SellingView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _settings = ((App)Application.Current).Services.GetService<UserSettingsService>();
        if (_settings is not null && _settings.Settings.CartPanelWidth > 0)
        {
            CartColumn.Width = new GridLength(
                Math.Clamp(_settings.Settings.CartPanelWidth, CartColumn.MinWidth, CartColumn.MaxWidth));
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_settings is not null)
        {
            _settings.Settings.CartPanelWidth = CartColumn.ActualWidth;
            _settings.Save();
        }
    }
}
