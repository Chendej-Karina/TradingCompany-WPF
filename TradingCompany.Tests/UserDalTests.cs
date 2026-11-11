using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class UserDalTests : TestBase
{
    private UserDal Dal => new(Context);

    [Fact]
    public void GetByLogin_Manager_HasRole()
    {
        var user = Dal.GetByLogin("manager");
        Assert.NotNull(user);
        Assert.Equal("AuctionManager", user!.RoleName);
    }

    [Fact]
    public void GetByLogin_Guest_HasNoRole()
    {
        var user = Dal.GetByLogin("guest");
        Assert.NotNull(user);
        Assert.Null(user!.RoleName);
        Assert.Null(user.RoleId);
    }

    [Fact]
    public void GetByLogin_Unknown_ReturnsNull() => Assert.Null(Dal.GetByLogin("nobody-xyz"));

    [Fact]
    public void Add_And_GetById_Works()
    {
        var u = new UserDto { Login = "test_user", PasswordHash = "hash", RoleId = null };
        Dal.Add(u);
        Assert.Equal("test_user", Dal.GetById(u.Id)!.Login);
    }

    [Fact]
    public void Update_ChangesRole()
    {
        var u = new UserDto { Login = "test_user2", PasswordHash = "hash" };
        Dal.Add(u);
        u.RoleId = Context.Roles.First(r => r.Name == "Viewer").Id;
        Dal.Update(u);
        Assert.Equal("Viewer", Dal.GetById(u.Id)!.RoleName);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var u = new UserDto { Login = "test_user3", PasswordHash = "hash" };
        Dal.Add(u);
        Dal.Delete(u.Id);
        Assert.Null(Dal.GetById(u.Id));
    }
}
