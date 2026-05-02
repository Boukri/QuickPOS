using System.Windows;
using System.Windows.Media.Animation;

namespace QuickPOS;

public partial class MainWindow : Window
{
    private const double ExpandedWidth = 180;
    private const double CollapsedWidth = 64;
    private static readonly Duration AnimDuration = new(TimeSpan.FromMilliseconds(200));

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        NavToggle.Checked += (_, _) => AnimateNav(CollapsedWidth);
        NavToggle.Unchecked += (_, _) => AnimateNav(ExpandedWidth);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        Application.Current.Shutdown();
    }

    private void AnimateNav(double targetWidth)
    {
        var widthAnim = new DoubleAnimation(targetWidth, AnimDuration)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };
        SideNav.BeginAnimation(WidthProperty, widthAnim);

        var opacityAnim = new DoubleAnimation(targetWidth > CollapsedWidth ? 1 : 0, AnimDuration)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };
        LogoSubtitle.BeginAnimation(OpacityProperty, opacityAnim);
    }
}
