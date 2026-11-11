using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;

public class LoginViewModel : ValidatableViewModelBase
{
    private readonly IAuthService _auth;
    private readonly IUserSession _session;
    private readonly IPermissionService _permissions;
    private readonly INavigationService _navigation;

    private string _login = string.Empty;
    private string _password = string.Empty;
    private string? _errorMessage;

    public LoginViewModel(IAuthService auth, IUserSession session, IPermissionService permissions,
        INavigationService navigation)
    {
        _auth = auth;
        _session = session;
        _permissions = permissions;
        _navigation = navigation;
        LoginCommand = new AsyncRelayCommand(LoginAsync, onError: ex => ErrorMessage = "Помилка: " + ex.Message);
    }

    public string Login
    {
        get => _login;
        set
        {
            if (SetProperty(ref _login, value)) ValidateLogin();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value)) ValidatePassword();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public AsyncRelayCommand LoginCommand { get; }

    private void ValidateLogin() =>
        SetErrors(nameof(Login), string.IsNullOrWhiteSpace(_login) ? new[] { "Введіть логін." } : Array.Empty<string>());

    private void ValidatePassword() =>
        SetErrors(nameof(Password), string.IsNullOrEmpty(_password) ? new[] { "Введіть пароль." } : Array.Empty<string>());

    private async Task LoginAsync()
    {
        ErrorMessage = null;
        ValidateLogin();
        ValidatePassword();
        if (HasErrors) return;

        var result = await Task.Run(() => _auth.Login(Login, Password));
        if (!result.Success || result.Value == null)
        {
            ErrorMessage = result.Error;
            return;
        }

        _session.SignIn(result.Value);
        _password = string.Empty;
        OnPropertyChanged(nameof(Password));

        var hasAnyAccess = _permissions.GetPermissions(_session.Role).Count > 0;
        if (hasAnyAccess) _navigation.NavigateTo<HomeViewModel>();
        else _navigation.NavigateTo<NoAccessViewModel>();
    }
}
