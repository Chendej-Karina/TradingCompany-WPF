namespace TradingCompany.ViewModels.Basic;


public interface IAsyncInitializable
{
    Task InitializeAsync();
}

public interface INavigationService
{
    ObservableObject? Current { get; }
    event Action? Navigated;

    
    void NavigateTo<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : ObservableObject;
}
