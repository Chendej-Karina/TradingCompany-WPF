using TradingCompany.BLL.Security;

namespace TradingCompany.Tests.BLL;

public class PermissionServiceTests
{
    private readonly PermissionService _service = new();

    [Theory]
    [InlineData(Permission.ViewItems)]
    [InlineData(Permission.ViewAuctions)]
    [InlineData(Permission.ManageAuctions)]
    public void AuctionManager_HasAllPermissions(Permission permission)
    {
        Assert.True(_service.HasPermission(RoleNames.AuctionManager, permission));
    }

    [Fact]
    public void Viewer_CanOnlyView()
    {
        Assert.True(_service.HasPermission(RoleNames.Viewer, Permission.ViewItems));
        Assert.True(_service.HasPermission(RoleNames.Viewer, Permission.ViewAuctions));
        Assert.False(_service.HasPermission(RoleNames.Viewer, Permission.ManageAuctions));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("UnknownRole")]
    public void NoRoleOrUnknownRole_HasNoPermissions(string? role)
    {
        Assert.Empty(_service.GetPermissions(role));
        Assert.False(_service.HasPermission(role, Permission.ViewItems));
    }

    [Fact]
    public void RoleName_IsCaseInsensitive()
    {
        Assert.True(_service.HasPermission("auctionmanager", Permission.ManageAuctions));
    }
}
