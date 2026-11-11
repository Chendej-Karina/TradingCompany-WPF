using TradingCompany.BLL.Security;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;


public class ShellViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IUserSession _session;
    private readonly IPermissionService _permissions;

    public ShellViewModel(INavigationService navigation, IUserSession session, IPermissionService permissions)
    {
        _navigation = navigation;
        _session = session;
        _permissions = permissions;

        _navigation.Navigated += () => OnPropertyChanged(nameof(CurrentViewModel));
        _session.Changed += RefreshMenu;

        HomeCommand = new RelayCommand(() => _navigation.NavigateTo<HomeViewModel>(), () => IsAuthenticated && HasAnyAccess);
        ItemsCommand = new RelayCommand(() => _navigation.NavigateTo<ItemListViewModel>(), () => CanViewItems);
        AuctionsCommand = new RelayCommand(() => _navigation.NavigateTo<AuctionListViewModel>(), () => CanViewAuctions);
        LogoutCommand = new RelayCommand(Logout, () => IsAuthenticated);
    }

    public ObservableObject? CurrentViewModel => _navigation.Current;

    public bool IsAuthenticated => _session.IsAuthenticated;
    public bool HasAnyAccess => _permissions.GetPermissions(_session.Role).Count > 0;
    public bool CanViewItems => _permissions.HasPermission(_session.Role, Permission.ViewItems);
    public bool CanViewAuctions => _permissions.HasPermission(_session.Role, Permission.ViewAuctions);
    public string UserText => _session.CurrentUser == null
        ? string.Empty
        : $"{_session.CurrentUser.Login} ({_session.Role ?? "без ролі"})";

    public RelayCommand HomeCommand { get; }
    public RelayCommand ItemsCommand { get; }
    public RelayCommand AuctionsCommand { get; }
    public RelayCommand LogoutCommand { get; }

    
    public void Start() => _navigation.NavigateTo<LoginViewModel>();

    private void Logout()
    {
        _session.SignOut();
        _navigation.NavigateTo<LoginViewModel>();
    }

    private void RefreshMenu()
    {
        OnPropertyChanged(nameof(IsAuthenticated));
        OnPropertyChanged(nameof(HasAnyAccess));
        OnPropertyChanged(nameof(CanViewItems));
        OnPropertyChanged(nameof(CanViewAuctions));
        OnPropertyChanged(nameof(UserText));
        HomeCommand.RaiseCanExecuteChanged();
        ItemsCommand.RaiseCanExecuteChanged();
        AuctionsCommand.RaiseCanExecuteChanged();
        LogoutCommand.RaiseCanExecuteChanged();
    }
}
