using TradingCompany.DAL.DTO;

namespace TradingCompany.BLL.Security;

public interface IUserSession
{
    UserDto? CurrentUser { get; }
    bool IsAuthenticated { get; }
    string? Role { get; }
    event Action? Changed;
    void SignIn(UserDto user);
    void SignOut();
}


public class UserSession : IUserSession
{
    public UserDto? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public string? Role => CurrentUser?.RoleName;
    public event Action? Changed;

    public void SignIn(UserDto user)
    {
        CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
        Changed?.Invoke();
    }

    public void SignOut()
    {
        CurrentUser = null;
        Changed?.Invoke();
    }
}
