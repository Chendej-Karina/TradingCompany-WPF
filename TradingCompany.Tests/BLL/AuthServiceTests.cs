using Moq;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.Tests.BLL;

public class AuthServiceTests
{
    private readonly Mock<IUserDal> _users = new();
    private readonly PasswordHasher _hasher = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_users.Object, _hasher);
        _users.Setup(u => u.GetByLogin("manager")).Returns(new UserDto
        {
            Id = 1,
            Login = "manager",
            PasswordHash = _hasher.Hash("Pass123!"),
            RoleId = 1,
            RoleName = RoleNames.AuctionManager
        });
    }

    [Fact]
    public void Login_CorrectCredentials_Succeeds()
    {
        var result = _service.Login("manager", "Pass123!");
        Assert.True(result.Success);
        Assert.Equal("manager", result.Value!.Login);
        Assert.Equal(RoleNames.AuctionManager, result.Value.RoleName);
    }

    [Fact]
    public void Login_TrimsLogin()
    {
        Assert.True(_service.Login("  manager  ", "Pass123!").Success);
    }

    [Fact]
    public void Login_WrongPassword_Fails()
    {
        var result = _service.Login("manager", "wrong");
        Assert.False(result.Success);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Login_UnknownUser_FailsWithSameMessage()
    {
        var wrongPassword = _service.Login("manager", "wrong");
        var unknownUser = _service.Login("nobody", "Pass123!");
        Assert.False(unknownUser.Success);
        Assert.Equal(wrongPassword.Error, unknownUser.Error);
    }

    [Theory]
    [InlineData(null, "x")]
    [InlineData("", "x")]
    [InlineData("   ", "x")]
    [InlineData("manager", null)]
    [InlineData("manager", "")]
    public void Login_EmptyInput_FailsWithoutQueryingDatabase(string? login, string? password)
    {
        var result = _service.Login(login, password);
        Assert.False(result.Success);
        _users.Verify(u => u.GetByLogin(It.IsAny<string>()), Times.Never);
    }
}
