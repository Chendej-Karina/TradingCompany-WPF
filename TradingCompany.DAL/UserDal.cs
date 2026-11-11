using Microsoft.EntityFrameworkCore;
using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class UserDal : DalBase<User, UserDto>, IUserDal
{
    public UserDal(TradingCompanyContext context) : base(context) { }

    protected override IQueryable<User> Query => _set.Include(u => u.Role);

    protected override int GetId(User e) => e.Id;

    protected override UserDto ToDto(User e) => new()
    {
        Id = e.Id,
        Login = e.Login,
        PasswordHash = e.PasswordHash,
        RoleId = e.RoleId,
        RoleName = e.Role?.Name
    };

    protected override void Apply(UserDto d, User e)
    {
        e.Login = d.Login;
        e.PasswordHash = d.PasswordHash;
        e.RoleId = d.RoleId;
    }

    public UserDto? GetByLogin(string login)
    {
        var user = Query.AsNoTracking().FirstOrDefault(u => u.Login == login);
        return user == null ? null : ToDto(user);
    }
}
