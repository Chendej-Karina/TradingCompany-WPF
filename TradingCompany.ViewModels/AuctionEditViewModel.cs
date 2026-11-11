using System.Collections.ObjectModel;
using TradingCompany.BLL.Common;
using TradingCompany.BLL.Services;
using TradingCompany.BLL.Validation;
using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;


public class AuctionEditViewModel : ValidatableViewModelBase, IAsyncInitializable
{
    private static readonly string[] Tracked =
    {
        nameof(CurrencyId), nameof(StartDate), nameof(EndDate), nameof(StartPrice), nameof(BuyoutPrice)
    };

    private readonly IAuctionService _auctions;
    private readonly IAuctionValidator _validator;
    private readonly INavigationService _navigation;
    private readonly HashSet<string> _touched = new();

    private int _itemId;
    private string _itemName = string.Empty;
    private int _currencyId;
    private DateTime? _startDate = DateTime.Today;
    private DateTime? _endDate = DateTime.Today.AddDays(7);
    private decimal? _startPrice;
    private decimal? _buyoutPrice;
    private string? _errorMessage;

    public AuctionEditViewModel(IAuctionService auctions, IAuctionValidator validator, INavigationService navigation)
    {
        _auctions = auctions;
        _validator = validator;
        _navigation = navigation;

        SaveCommand = new AsyncRelayCommand(SaveAsync, onError: ex => ErrorMessage = ex.Message);
        CancelCommand = new RelayCommand(() => _navigation.NavigateTo<ItemListViewModel>());
    }

    public ObservableCollection<CurrencyDto> Currencies { get; } = new();

    public int ItemId
    {
        get => _itemId;
        private set => SetProperty(ref _itemId, value);
    }

    public string ItemName
    {
        get => _itemName;
        private set => SetProperty(ref _itemName, value);
    }

    public int CurrencyId
    {
        get => _currencyId;
        set => SetProperty(ref _currencyId, value);
    }

    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public decimal? StartPrice
    {
        get => _startPrice;
        set => SetProperty(ref _startPrice, value);
    }

    public decimal? BuyoutPrice
    {
        get => _buyoutPrice;
        set => SetProperty(ref _buyoutPrice, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public AsyncRelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

   
    public void Initialize(ItemDto item)
    {
        ItemId = item.Id;
        ItemName = item.Name;
    }

    public async Task InitializeAsync()
    {
        OperationResult<IReadOnlyList<CurrencyDto>> result;
        try { result = await Task.Run(() => _auctions.GetCurrencies()); }
        catch (Exception ex) { ErrorMessage = ex.Message; return; }
        if (!result.Success || result.Value == null)
        {
            ErrorMessage = result.Error;
            return;
        }

        Currencies.Clear();
        foreach (var c in result.Value) Currencies.Add(c);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName != null && Tracked.Contains(propertyName))
        {
            _touched.Add(propertyName);
            Validate(showAll: false);
        }
    }

    private AuctionDto BuildDto() => new()
    {
        ItemId = ItemId,
        CurrencyId = CurrencyId,
        StartDate = StartDate ?? default,
        EndDate = EndDate ?? default,
        StartPrice = StartPrice ?? 0,
        BuyoutPrice = BuyoutPrice
    };

    /// <summary>Перевіряє дані. showAll = true показує помилки всіх полів, інакше лише вже змінених.</summary>
    private void Validate(bool showAll)
    {
        var errors = _validator.Validate(BuildDto());
        foreach (var property in Tracked)
        {
            if (!showAll && !_touched.Contains(property))
            {
                ClearErrors(property);
                continue;
            }

            SetErrors(property, errors.Where(e => e.Property == property).Select(e => e.Message));
        }

        var general = errors.FirstOrDefault(e => e.Property == nameof(AuctionDto.ItemId));
        ErrorMessage = general?.Message;
    }

    private async Task SaveAsync()
    {
        ErrorMessage = null;
        Validate(showAll: true);
        if (HasErrors || ErrorMessage != null) return;

        var dto = BuildDto();
        var result = await Task.Run(() => _auctions.Create(dto));
        if (!result.Success)
        {
            ErrorMessage = result.Error;
            return;
        }

        _navigation.NavigateTo<AuctionListViewModel>();
    }
}
