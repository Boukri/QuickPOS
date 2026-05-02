using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuickPOS.Features.Selling;

public class PaymentDialogViewModel : ObservableObject, IDisposable
{
    public decimal TotalAmount { get; }

    private string _clientBillText = string.Empty;
    public string ClientBillText
    {
        get => _clientBillText;
        set
        {
            if (SetProperty(ref _clientBillText, value))
            {
                OnPropertyChanged(nameof(ClientBill));
                OnPropertyChanged(nameof(Change));
                OnPropertyChanged(nameof(IsInsufficient));
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public decimal ClientBill =>
        decimal.TryParse(_clientBillText, out var v) ? v : 0m;

    public decimal Change => ClientBill - TotalAmount;

    public bool IsInsufficient => _clientBillText.Length > 0 && ClientBill < TotalAmount;

    public IRelayCommand ConfirmCommand { get; }
    public IRelayCommand<string> NumpadPressCommand { get; }
    public IRelayCommand NumpadBackspaceCommand { get; }
    public IRelayCommand NumpadClearCommand { get; }

    public event EventHandler? PaymentConfirmed;
    private bool isFirstTap = true;
    public PaymentDialogViewModel(decimal totalAmount)
    {
        TotalAmount = totalAmount;
        _clientBillText = totalAmount.ToString("F2");

        ConfirmCommand = new RelayCommand(
            execute: () => PaymentConfirmed?.Invoke(this, EventArgs.Empty),
            canExecute: () => ClientBill >= TotalAmount);

        NumpadPressCommand = new RelayCommand<string>(digit =>
        {
            if (isFirstTap)
            {
                ClientBillText = "";
                isFirstTap = false;
            }
           
            if (digit == ".")
            {
                // Allow only one decimal separator
                if (!_clientBillText.Contains('.'))
                    ClientBillText = _clientBillText + ".";
            }
            else
            {
                // Replace leading zero unless it's followed by a decimal
                var current = _clientBillText == "0" ? string.Empty : _clientBillText;
                ClientBillText = current + digit;
            }
        });

        NumpadBackspaceCommand = new RelayCommand(() =>
        {
            if (_clientBillText.Length > 0)
                ClientBillText = _clientBillText[..^1];
        });

        NumpadClearCommand = new RelayCommand(() => ClientBillText = string.Empty);
    }

    public void Dispose()
    {
        PaymentConfirmed = null;
    }
}
