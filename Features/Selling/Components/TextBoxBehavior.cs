using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuickPOS.Features.Selling.Components
{
    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty SelectAllOnLoadProperty =
            DependencyProperty.RegisterAttached("SelectAllOnLoad", typeof(bool), typeof(TextBoxBehavior),
                new PropertyMetadata(false, OnSelectAllOnLoadChanged));

        public static void SetSelectAllOnLoad(DependencyObject obj, bool value) => obj.SetValue(SelectAllOnLoadProperty, value);
        public static bool GetSelectAllOnLoad(DependencyObject obj) => (bool)obj.GetValue(SelectAllOnLoadProperty);

        private static void OnSelectAllOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                textBox.Loaded += (_, _) =>
                {
                    // Defer until after Window activation and OS focus assignment
                    textBox.Dispatcher.BeginInvoke(DispatcherPriority.Input, () =>
                    {
                        textBox.Focus();
                        textBox.SelectAll();
                    });
                };
            }
        }
    }
}
