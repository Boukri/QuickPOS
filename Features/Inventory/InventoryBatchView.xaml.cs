using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using QuickPOS.Features.Inventory.Components;

namespace QuickPOS.Features.Inventory;

public partial class InventoryBatchView : UserControl
{
    private InventoryBatchViewModel? _vm;

    public InventoryBatchView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_vm is not null)
        {
            _vm.ReceiveStockRequested -= OnReceiveStockRequested;
        }

        _vm = e.NewValue as InventoryBatchViewModel;

        if (_vm is not null)
        {
            _vm.ReceiveStockRequested += OnReceiveStockRequested;
        }
    }

    private void OnReceiveStockRequested(object? sender, EventArgs e)
    {
        var dialog = new ReceiveStockDialog(_vm!) { Owner = Window.GetWindow(this) };
        dialog.ShowDialog();
    }
}
