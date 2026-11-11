using TradingCompany.BLL.Security;
using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.Tests;


public class FakeNavigationService : INavigationService
{
    public ObservableObject? Current => null;
    public event Action? Navigated;
    public Type? LastTarget { get; private set; }
    public int Count { get; private set; }

    public void NavigateTo<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : ObservableObject
    {
        LastTarget = typeof(TViewModel);
        Count++;
        Navigated?.Invoke();
    }
}

public static class TestUsers
{
    public static UserDto User(string login = "user", string? role = null) =>
        new() { Id = 1, Login = login, PasswordHash = "x", RoleName = role, RoleId = role == null ? null : 1 };

    \
    public static UserSession Session(string? role)
    {
        var session = new UserSession();
        session.SignIn(User(role: role));
        return session;
    }
}
