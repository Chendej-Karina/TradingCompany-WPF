using System.Collections.ObjectModel;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;

public enum AuctionFilter
{
    Active,
    Finished
}


public class AuctionListViewModel : ObservableObject, IAsyncInitializable
{
    private readonly IAuctionService _auctions;
    private readonly bool _canManage;

    private AuctionFilter _filter = AuctionFilter.Active;
    private AuctionListItemViewModel? _selectedAuction;
    private string? _errorMessage;
    private bool _initialized;

    public AuctionListViewModel(IAuctionService auctions, IUserSession session, IPermissionService permissions)
    {
        _auctions = auctions;
        _canManage = permissions.HasPermission(session.Role, Permission.ManageAuctions);

        RefreshCommand = new AsyncRelayCommand(LoadAsync, onError: ex => ErrorMessage = ex.Message);
        DeactivateCommand = new AsyncRelayCommand(DeactivateAsync, CanDeactivate, ex => ErrorMessage = ex.Message);
    }

    public ObservableCollection<AuctionListItemViewModel> Auctions { get; } = new();

    public AuctionFilter Filter
    {
        get => _filter;
        set
        {
            if (!SetProperty(ref _filter, value)) return;
            
            if (_initialized) _ = RefreshCommand.ExecuteAsync();
        }
    }

    public AuctionListItemViewModel? SelectedAuction
    {
        get => _selectedAuction;
        set
        {
            if (SetProperty(ref _selectedAuction, value)) DeactivateCommand.RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool CanManage => _canManage;

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand DeactivateCommand { get; }

    public async Task InitializeAsync()
    {
        try { await LoadAsync(); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        _initialized = true;
    }

    public async Task LoadAsync()
    {
        ErrorMessage = null;
        var filter = Filter;

        var result = await Task.Run(() => filter == AuctionFilter.Active
            ? _auctions.GetActive()
            : _auctions.GetFinished());

        if (!result.Success || result.Value == null)
        {
            ErrorMessage = result.Error;
            return;
        }

        Auctions.Clear();
        foreach (var a in result.Value) Auctions.Add(new AuctionListItemViewModel(a));
        SelectedAuction = null;
    }

    private bool CanDeactivate() => _canManage && SelectedAuction is { Auction.IsActive: true };

    private async Task DeactivateAsync()
    {
        var selected = SelectedAuction;
        if (selected == null) return;

        var result = await Task.Run(() => _auctions.Deactivate(selected.Id));
        if (!result.Success)
        {
            ErrorMessage = result.Error;
            return;
        }

        await LoadAsync();
    }
}
