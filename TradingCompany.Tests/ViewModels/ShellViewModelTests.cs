using TradingCompany.BLL.Security;
using TradingCompany.ViewModels;

namespace TradingCompany.Tests.ViewModels;

public class ShellViewModelTests
{
    private readonly FakeNavigationService _navigation = new();

    [Fact]
    public void Start_OpensLoginPage()
    {
        var shell = new ShellViewModel(_navigation, new UserSession(), new PermissionService());
        shell.Start();
        Assert.Equal(typeof(LoginViewModel), _navigation.LastTarget);
    }

    [Fact]
    public void Menu_IsHiddenAndDisabledBeforeLogin()
    {
        var shell = new ShellViewModel(_navigation, new UserSession(), new PermissionService());
        Assert.False(shell.IsAuthenticated);
        Assert.False(shell.ItemsCommand.CanExecute(null));
        Assert.False(shell.LogoutCommand.CanExecute(null));
    }

    [Fact]
    public void Manager_SeesAllMenuItems()
    {
        var session = new UserSession();
        var shell = new ShellViewModel(_navigation, session, new PermissionService());

        session.SignIn(TestUsers.User("manager", RoleNames.AuctionManager));

        Assert.True(shell.IsAuthenticated);
        Assert.True(shell.CanViewItems);
        Assert.True(shell.CanViewAuctions);
        Assert.True(shell.ItemsCommand.CanExecute(null));
        Assert.Contains("manager", shell.UserText);
    }

    [Fact]
    public void UserWithoutRole_HasNoAccessToMenuItems()
    {
        var session = new UserSession();
        var shell = new ShellViewModel(_navigation, session, new PermissionService());

        session.SignIn(TestUsers.User("guest", role: null));

        Assert.False(shell.HasAnyAccess);
        Assert.False(shell.ItemsCommand.CanExecute(null));
        Assert.False(shell.AuctionsCommand.CanExecute(null));
        Assert.False(shell.HomeCommand.CanExecute(null));
    }

    [Fact]
    public void Logout_SignsOutAndReturnsToLogin()
    {
        var session = new UserSession();
        var shell = new ShellViewModel(_navigation, session, new PermissionService());
        session.SignIn(TestUsers.User("manager", RoleNames.AuctionManager));

        shell.LogoutCommand.Execute(null);

        Assert.False(session.IsAuthenticated);
        Assert.Equal(typeof(LoginViewModel), _navigation.LastTarget);
    }
}
