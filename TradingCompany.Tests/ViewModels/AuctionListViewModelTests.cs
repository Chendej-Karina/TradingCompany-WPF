using Moq;
using TradingCompany.BLL.Common;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels;

namespace TradingCompany.Tests.ViewModels;

public class AuctionListViewModelTests
{
    private readonly Mock<IAuctionService> _service = new();

    private static OperationResult<IReadOnlyList<AuctionDto>> Ok(params AuctionDto[] auctions) =>
        OperationResult<IReadOnlyList<AuctionDto>>.Ok(auctions.ToList());

    private static AuctionDto Active(int id) => new()
    {
        Id = id, IsActive = true, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(3),
        ItemName = "Товар", CurrencyCode = "UAH", StartPrice = 10
    };

    private AuctionListViewModel Create(string role) =>
        new(_service.Object, TestUsers.Session(role), new PermissionService());

    [Fact]
    public async Task InitializeAsync_LoadsActiveAuctions()
    {
        _service.Setup(s => s.GetActive()).Returns(Ok(Active(1), Active(2)));
        var vm = Create(RoleNames.AuctionManager);

        await vm.InitializeAsync();

        Assert.Equal(2, vm.Auctions.Count);
        Assert.Equal(AuctionFilter.Active, vm.Filter);
    }

    [Fact]
    public async Task FinishedFilter_LoadsFinishedAuctions()
    {
        _service.Setup(s => s.GetFinished()).Returns(Ok(new AuctionDto { Id = 9, IsActive = false }));
        var vm = Create(RoleNames.AuctionManager);

        vm.Filter = AuctionFilter.Finished;
        await vm.LoadAsync();

        Assert.Single(vm.Auctions);
        Assert.Equal(9, vm.Auctions[0].Id);
        _service.Verify(s => s.GetActive(), Times.Never);
    }

    [Fact]
    public async Task ServiceError_IsShown_AndListStaysEmpty()
    {
        _service.Setup(s => s.GetActive())
            .Returns(OperationResult<IReadOnlyList<AuctionDto>>.Fail("Немає доступу."));
        var vm = Create(RoleNames.Viewer);

        await vm.LoadAsync();

        Assert.Equal("Немає доступу.", vm.ErrorMessage);
        Assert.Empty(vm.Auctions);
    }

    [Fact]
    public async Task DeactivateCommand_ManagerWithSelection_DeactivatesAndReloads()
    {
        _service.SetupSequence(s => s.GetActive()).Returns(Ok(Active(1))).Returns(Ok());
        _service.Setup(s => s.Deactivate(1)).Returns(OperationResult.Ok());
        var vm = Create(RoleNames.AuctionManager);
        await vm.LoadAsync();
        vm.SelectedAuction = vm.Auctions[0];

        Assert.True(vm.DeactivateCommand.CanExecute(null));
        await vm.DeactivateCommand.ExecuteAsync();

        _service.Verify(s => s.Deactivate(1), Times.Once);
        Assert.Empty(vm.Auctions);
    }

    [Fact]
    public async Task DeactivateCommand_Viewer_IsNotAvailable()
    {
        _service.Setup(s => s.GetActive()).Returns(Ok(Active(1)));
        var vm = Create(RoleNames.Viewer);
        await vm.LoadAsync();
        vm.SelectedAuction = vm.Auctions[0];

        Assert.False(vm.CanManage);
        Assert.False(vm.DeactivateCommand.CanExecute(null));
    }

    [Fact]
    public void DeactivateCommand_NoSelection_IsNotAvailable()
    {
        Assert.False(Create(RoleNames.AuctionManager).DeactivateCommand.CanExecute(null));
    }

    [Fact]
    public async Task DeactivateFails_ShowsError()
    {
        _service.Setup(s => s.GetActive()).Returns(Ok(Active(1)));
        _service.Setup(s => s.Deactivate(1)).Returns(OperationResult.Fail("Помилка."));
        var vm = Create(RoleNames.AuctionManager);
        await vm.LoadAsync();
        vm.SelectedAuction = vm.Auctions[0];

        await vm.DeactivateCommand.ExecuteAsync();

        Assert.Equal("Помилка.", vm.ErrorMessage);
    }

    [Fact]
    public void ListItem_StatusText_ReflectsState()
    {
        var deactivated = new AuctionListItemViewModel(new AuctionDto { IsActive = false, EndDate = DateTime.Now.AddDays(1) });
        var ended = new AuctionListItemViewModel(new AuctionDto { IsActive = true, EndDate = DateTime.Now.AddDays(-1) });
        var open = new AuctionListItemViewModel(Active(1));

        Assert.Equal("Деактивований", deactivated.StatusText);
        Assert.Equal("Завершений", ended.StatusText);
        Assert.Equal("Активний", open.StatusText);
        Assert.True(open.IsOpen);
        Assert.False(ended.IsOpen);
    }
}
