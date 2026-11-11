namespace TradingCompany.BLL.Security;


public enum Permission
{
    ViewItems,
    ViewAuctions,
    ManageAuctions
}


public static class RoleNames
{
    public const string AuctionManager = "AuctionManager";
    public const string Viewer = "Viewer";
}
