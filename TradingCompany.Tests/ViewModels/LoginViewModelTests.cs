using Moq;
using TradingCompany.BLL.Common;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels;

namespace TradingCompany.Tests.ViewModels;

public class LoginViewModelTests
{
    private readonly Mock<IAuthService> _auth = new();
    private readonly UserSession _session = new();
    private readonly FakeNavigationService _navigation = new();

    private LoginViewModel Create() =>
        new(_auth.Object, _session, new PermissionService(), _navigation);

    [Fact]
    public async Task Login_EmptyFields_ShowsValidationErrors_AndDoesNotCallService()
    {
        var vm = Create();

        await vm.LoginCommand.ExecuteAsync();

        Assert.True(vm.HasErrors);
        Assert.NotEmpty(vm.GetErrors(nameof(vm.Login)).Cast<string>());
        Assert.NotEmpty(vm.GetErrors(nameof(vm.Password)).Cast<string>());
        _auth.Verify(a => a.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        Assert.False(_session.IsAuthenticated);
    }

    [Fact]
    public void ChangingLoginToValue_ClearsItsError()
    {
        var vm = Create();
        vm.Login = "";
        Assert.NotEmpty(vm.GetErrors(nameof(vm.Login)).Cast<string>());

        vm.Login = "manager";

        Assert.Empty(vm.GetErrors(nameof(vm.Login)).Cast<string>());
    }

    [Fact]
    public async Task Login_WrongCredentials_ShowsMessage_AndStaysOnPage()
    {
        _auth.Setup(a => a.Login("manager", "bad")).Returns(OperationResult<UserDto>.Fail("Невірний логін або пароль."));
        var vm = Create();
        vm.Login = "manager";
        vm.Password = "bad";

        await vm.LoginCommand.ExecuteAsync();

        Assert.Equal("Невірний логін або пароль.", vm.ErrorMessage);
        Assert.False(_session.IsAuthenticated);
        Assert.Equal(0, _navigation.Count);
    }

    [Fact]
    public async Task Login_UserWithRole_SignsInAndOpensHome()
    {
        var user = TestUsers.User("manager", RoleNames.AuctionManager);
        _auth.Setup(a => a.Login("manager", "ok")).Returns(OperationResult<UserDto>.Ok(user));
        var vm = Create();
        vm.Login = "manager";
        vm.Password = "ok";

        await vm.LoginCommand.ExecuteAsync();

        Assert.True(_session.IsAuthenticated);
        Assert.Equal(typeof(HomeViewModel), _navigation.LastTarget);
        Assert.Equal(string.Empty, vm.Password);
        Assert.Null(vm.ErrorMessage);
    }

    [Fact]
    public async Task Login_UserWithoutRole_OpensNoAccessPage()
    {
        var user = TestUsers.User("guest", role: null);
        _auth.Setup(a => a.Login("guest", "ok")).Returns(OperationResult<UserDto>.Ok(user));
        var vm = Create();
        vm.Login = "guest";
        vm.Password = "ok";

        await vm.LoginCommand.ExecuteAsync();

        Assert.True(_session.IsAuthenticated);
        Assert.Equal(typeof(NoAccessViewModel), _navigation.LastTarget);
    }

    [Fact]
    public async Task Login_ServiceThrows_ShowsErrorInsteadOfCrashing()
    {
        _auth.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("БД недоступна"));
        var vm = Create();
        vm.Login = "manager";
        vm.Password = "x";

        await vm.LoginCommand.ExecuteAsync();

        Assert.Contains("БД недоступна", vm.ErrorMessage);
    }
}
