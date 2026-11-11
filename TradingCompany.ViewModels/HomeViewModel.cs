using TradingCompany.BLL.Security;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;


public class HomeViewModel : ObservableObject
{
    public HomeViewModel(IUserSession session, IPermissionService permissions, INavigationService navigation)
    {
        var role = session.Role;
        Greeting = $"Вітаємо, {session.CurrentUser?.Login}!";
        RoleText = role == RoleNames.AuctionManager ? "Роль: Менеджер аукціонів" : $"Роль: {role}";

        CanViewItems = permissions.HasPermission(role, Permission.ViewItems);
        CanViewAuctions = permissions.HasPermission(role, Permission.ViewAuctions);

        OpenItemsCommand = new RelayCommand(() => navigation.NavigateTo<ItemListViewModel>(), () => CanViewItems);
        OpenAuctionsCommand = new RelayCommand(() => navigation.NavigateTo<AuctionListViewModel>(), () => CanViewAuctions);
    }

    public string Greeting { get; }
    public string RoleText { get; }
    public bool CanViewItems { get; }
    public bool CanViewAuctions { get; }
    public RelayCommand OpenItemsCommand { get; }
    public RelayCommand OpenAuctionsCommand { get; }
}
