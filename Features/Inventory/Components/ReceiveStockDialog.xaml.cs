using System.Windows;

namespace QuickPOS.Features.Inventory.Components;

public partial class ReceiveStockDialog : Window
{
    private readonly InventoryBatchViewModel _vm;

    public ReceiveStockDialog(InventoryBatchViewModel vm)
    {
        InitializeComponent();
        DataContext = _vm = vm;
        _vm.ReceiveStockCompleted += OnReceiveStockCompleted;
        Closed += (_, _) => _vm.ReceiveStockCompleted -= OnReceiveStockCompleted;
    }

    private void OnReceiveStockCompleted(object? sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
