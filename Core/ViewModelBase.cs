using CommunityToolkit.Mvvm.ComponentModel;

namespace QuickPOS.Core;

public abstract class ViewModelBase : ObservableObject
{
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }
}
