using TradingCompany.BLL.Common;
using TradingCompany.BLL.Security;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.BLL.Services;

public interface IAuthService
{
    OperationResult<UserDto> Login(string? login, string? password);
}

public class AuthService : IAuthService
{
    private readonly IUserDal _users;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUserDal users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public OperationResult<UserDto> Login(string? login, string? password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            return OperationResult<UserDto>.Fail("Введіть логін і пароль.");

        var user = _users.GetByLogin(login.Trim());
        
        if (user == null || !_hasher.Verify(password, user.PasswordHash))
            return OperationResult<UserDto>.Fail("Невірний логін або пароль.");

        return OperationResult<UserDto>.Ok(user);
    }
}
