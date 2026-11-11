using System.Collections.ObjectModel;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;

public record SortOption(ItemSortField Field, string Title);


public class ItemListViewModel : ObservableObject, IAsyncInitializable
{
    private readonly IItemService _items;
    private readonly INavigationService _navigation;
    private readonly bool _canManageAuctions;

    private string _searchText = string.Empty;
    private ItemSortField _sortBy = ItemSortField.Id;
    private bool _descending;
    private ItemRowViewModel? _selectedItem;
    private string? _errorMessage;

    public ItemListViewModel(IItemService items, IUserSession session, IPermissionService permissions,
        INavigationService navigation)
    {
        _items = items;
        _navigation = navigation;
        _canManageAuctions = permissions.HasPermission(session.Role, Permission.ManageAuctions);

        SearchCommand = new AsyncRelayCommand(LoadAsync, onError: ex => ErrorMessage = ex.Message);
        ClearSearchCommand = new AsyncRelayCommand(async () =>
        {
            SearchText = string.Empty;
            await LoadAsync();
        });
        CreateAuctionCommand = new RelayCommand(CreateAuction, CanCreateAuction);
    }

    public ObservableCollection<ItemRowViewModel> Items { get; } = new();

    public IReadOnlyList<SortOption> SortOptions { get; } = new[]
    {
        new SortOption(ItemSortField.Id, "За номером"),
        new SortOption(ItemSortField.Name, "За назвою"),
        new SortOption(ItemSortField.Category, "За категорією")
    };

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public ItemSortField SortBy
    {
        get => _sortBy;
        set => SetProperty(ref _sortBy, value);
    }

    public bool Descending
    {
        get => _descending;
        set => SetProperty(ref _descending, value);
    }

    public ItemRowViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value)) CreateAuctionCommand.RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool CanManageAuctions => _canManageAuctions;

    public AsyncRelayCommand SearchCommand { get; }
    public AsyncRelayCommand ClearSearchCommand { get; }
    public RelayCommand CreateAuctionCommand { get; }

    public async Task InitializeAsync()
    {
        try { await LoadAsync(); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    public async Task LoadAsync()
    {
        ErrorMessage = null;
        var search = SearchText;
        var sortBy = SortBy;
        var descending = Descending;

        var result = await Task.Run(() => _items.GetItems(search, sortBy, descending));
        if (!result.Success || result.Value == null)
        {
            ErrorMessage = result.Error;
            return;
        }

        Items.Clear();
        foreach (var item in result.Value) Items.Add(new ItemRowViewModel(item));
        SelectedItem = null;
    }

    private bool CanCreateAuction() => _canManageAuctions && SelectedItem is { IsActive: true };

    private void CreateAuction()
    {
        var item = SelectedItem;
        if (item == null) return;
        _navigation.NavigateTo<AuctionEditViewModel>(vm => vm.Initialize(item.Item));
    }
}
