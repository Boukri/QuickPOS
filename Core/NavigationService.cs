using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace QuickPOS.Core;

public partial class NavigationService : ObservableObject
{
    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    private readonly IServiceProvider _serviceProvider;
    private IServiceScope? _currentScope;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    public void NavigateTo(Type viewModelType)
    {
        _currentScope?.Dispose();
        _currentScope = _serviceProvider.CreateScope();
        CurrentViewModel = (ViewModelBase)_currentScope.ServiceProvider.GetRequiredService(viewModelType);
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        NavigateTo(typeof(TViewModel));
    }
}
