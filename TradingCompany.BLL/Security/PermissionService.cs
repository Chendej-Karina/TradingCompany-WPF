namespace TradingCompany.BLL.Security;

public interface IPermissionService
{
    bool HasPermission(string? role, Permission permission);
    IReadOnlyCollection<Permission> GetPermissions(string? role);
}


public class PermissionService : IPermissionService
{
    private static readonly Dictionary<string, Permission[]> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        [RoleNames.AuctionManager] = new[] { Permission.ViewItems, Permission.ViewAuctions, Permission.ManageAuctions },
        [RoleNames.Viewer] = new[] { Permission.ViewItems, Permission.ViewAuctions }
    };

    public IReadOnlyCollection<Permission> GetPermissions(string? role) =>
        role != null && Map.TryGetValue(role, out var permissions)
            ? permissions
            : Array.Empty<Permission>();

    public bool HasPermission(string? role, Permission permission) =>
        GetPermissions(role).Contains(permission);
}
