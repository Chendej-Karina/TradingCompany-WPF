using TradingCompany.DAL.DTO;

namespace TradingCompany.DAL.Interfaces;

public interface IUserDal : ICrudDal<UserDto>
{
    UserDto? GetByLogin(string login);
}
