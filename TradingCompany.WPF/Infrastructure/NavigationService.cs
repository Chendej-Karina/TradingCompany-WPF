using Microsoft.Extensions.DependencyInjection;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.WPF.Infrastructure;


public class NavigationService : INavigationService
{
    private readonly IServiceProvider _provider;
    private IServiceScope? _scope;

    public NavigationService(IServiceProvider provider) => _provider = provider;

    public ObservableObject? Current { get; private set; }

    public event Action? Navigated;

    public void NavigateTo<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : ObservableObject
    {
        var newScope = _provider.CreateScope();
        var viewModel = newScope.ServiceProvider.GetRequiredService<TViewModel>();
        configure?.Invoke(viewModel);

        var oldScope = _scope;
        _scope = newScope;
        Current = viewModel;
        Navigated?.Invoke();
        oldScope?.Dispose();

        if (viewModel is IAsyncInitializable initializable)
            _ = initializable.InitializeAsync();
    }
}
