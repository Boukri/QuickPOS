using System.Windows;
using System.Windows.Controls;

namespace QuickPOS.Features.Selling;

public partial class PaymentDialog : Window
{
    private readonly PaymentDialogViewModel _viewModel;

    public PaymentDialog(PaymentDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;

        viewModel.PaymentConfirmed += (_, _) =>
        {
            DialogResult = true;
            Close();
        };

        Closed += (_, _) => _viewModel.Dispose();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
