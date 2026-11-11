using Moq;
using TradingCompany.BLL.Common;
using TradingCompany.BLL.Services;
using TradingCompany.BLL.Validation;
using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels;

namespace TradingCompany.Tests.ViewModels;

public class AuctionEditViewModelTests
{
    private readonly Mock<IAuctionService> _service = new();
    private readonly FakeNavigationService _navigation = new();

    private AuctionEditViewModel Create()
    {
        var vm = new AuctionEditViewModel(_service.Object, new AuctionValidator(), _navigation);
        vm.Initialize(new ItemDto { Id = 5, Name = "Годинник" });
        return vm;
    }

    private static void FillValid(AuctionEditViewModel vm)
    {
        vm.CurrencyId = 1;
        vm.StartDate = DateTime.Today;
        vm.EndDate = DateTime.Today.AddDays(7);
        vm.StartPrice = 100;
        vm.BuyoutPrice = 500;
    }

    [Fact]
    public void Initialize_SetsItem()
    {
        var vm = Create();
        Assert.Equal(5, vm.ItemId);
        Assert.Equal("Годинник", vm.ItemName);
    }

    [Fact]
    public void NewForm_HasNoErrorsUntilUserEditsFields()
    {
        var vm = Create();
        Assert.False(vm.HasErrors);
    }

    [Fact]
    public void NegativeStartPrice_ShowsErrorOnStartPrice()
    {
        var vm = Create();

        vm.StartPrice = -5;

        Assert.NotEmpty(vm.GetErrors(nameof(vm.StartPrice)).Cast<string>());
        Assert.True(vm.HasErrors);
    }

    [Fact]
    public void EndDateBeforeStart_ShowsErrorOnEndDate()
    {
        var vm = Create();
        vm.StartDate = DateTime.Today.AddDays(10);

        vm.EndDate = DateTime.Today.AddDays(3);

        Assert.NotEmpty(vm.GetErrors(nameof(vm.EndDate)).Cast<string>());
    }

    [Fact]
    public void FixingTheValue_ClearsTheError()
    {
        var vm = Create();
        vm.StartPrice = -5;

        vm.StartPrice = 100;

        Assert.Empty(vm.GetErrors(nameof(vm.StartPrice)).Cast<string>());
    }

    [Fact]
    public void BuyoutNotGreaterThanStart_ShowsErrorOnBuyout()
    {
        var vm = Create();
        vm.StartPrice = 100;

        vm.BuyoutPrice = 100;

        Assert.NotEmpty(vm.GetErrors(nameof(vm.BuyoutPrice)).Cast<string>());
    }

    [Fact]
    public async Task Save_InvalidForm_ShowsAllErrors_AndDoesNotCallService()
    {
        var vm = Create(); // не вибрано валюту, немає ціни

        await vm.SaveCommand.ExecuteAsync();

        Assert.True(vm.HasErrors);
        Assert.NotEmpty(vm.GetErrors(nameof(vm.CurrencyId)).Cast<string>());
        Assert.NotEmpty(vm.GetErrors(nameof(vm.StartPrice)).Cast<string>());
        _service.Verify(s => s.Create(It.IsAny<AuctionDto>()), Times.Never);
        Assert.Equal(0, _navigation.Count);
    }

    [Fact]
    public async Task Save_ValidForm_CreatesAuction_AndOpensAuctionList()
    {
        _service.Setup(s => s.Create(It.IsAny<AuctionDto>()))
            .Returns((AuctionDto a) => OperationResult<AuctionDto>.Ok(a));
        var vm = Create();
        FillValid(vm);

        await vm.SaveCommand.ExecuteAsync();

        _service.Verify(s => s.Create(It.Is<AuctionDto>(a =>
            a.ItemId == 5 && a.CurrencyId == 1 && a.StartPrice == 100 && a.BuyoutPrice == 500)), Times.Once);
        Assert.Equal(typeof(AuctionListViewModel), _navigation.LastTarget);
    }

    [Fact]
    public async Task Save_ServiceFails_ShowsMessage_AndStays()
    {
        _service.Setup(s => s.Create(It.IsAny<AuctionDto>()))
            .Returns(OperationResult<AuctionDto>.Fail("Немає прав."));
        var vm = Create();
        FillValid(vm);

        await vm.SaveCommand.ExecuteAsync();

        Assert.Equal("Немає прав.", vm.ErrorMessage);
        Assert.Equal(0, _navigation.Count);
    }

    [Fact]
    public void Cancel_ReturnsToItemList()
    {
        var vm = Create();
        vm.CancelCommand.Execute(null);
        Assert.Equal(typeof(ItemListViewModel), _navigation.LastTarget);
    }

    [Fact]
    public async Task InitializeAsync_LoadsCurrencies()
    {
        _service.Setup(s => s.GetCurrencies()).Returns(OperationResult<IReadOnlyList<CurrencyDto>>.Ok(
            new List<CurrencyDto> { new() { Id = 1, Code = "UAH" }, new() { Id = 2, Code = "USD" } }));
        var vm = Create();

        await vm.InitializeAsync();

        Assert.Equal(2, vm.Currencies.Count);
    }
}
