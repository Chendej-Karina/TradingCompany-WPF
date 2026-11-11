using Moq;
using TradingCompany.BLL.Common;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels;

namespace TradingCompany.Tests.ViewModels;

public class ItemListViewModelTests
{
    private readonly Mock<IItemService> _service = new();
    private readonly FakeNavigationService _navigation = new();

    private static OperationResult<IReadOnlyList<ItemDto>> Ok(params ItemDto[] items) =>
        OperationResult<IReadOnlyList<ItemDto>>.Ok(items.ToList());

    private ItemListViewModel Create(string role) =>
        new(_service.Object, TestUsers.Session(role), new PermissionService(), _navigation);

    [Fact]
    public async Task InitializeAsync_LoadsItems()
    {
        _service.Setup(s => s.GetItems("", ItemSortField.Id, false))
            .Returns(Ok(new ItemDto { Id = 1, Name = "А" }, new ItemDto { Id = 2, Name = "Б" }));
        var vm = Create(RoleNames.AuctionManager);

        await vm.InitializeAsync();

        Assert.Equal(2, vm.Items.Count);
    }

    [Fact]
    public async Task Search_PassesSearchTextAndSortingToService()
    {
        _service.Setup(s => s.GetItems(It.IsAny<string?>(), It.IsAny<ItemSortField>(), It.IsAny<bool>()))
            .Returns(Ok());
        var vm = Create(RoleNames.AuctionManager);
        vm.SearchText = "чай";
        vm.SortBy = ItemSortField.Name;
        vm.Descending = true;

        await vm.SearchCommand.ExecuteAsync();

        _service.Verify(s => s.GetItems("чай", ItemSortField.Name, true), Times.Once);
    }

    [Fact]
    public async Task ClearSearch_ResetsTextAndReloads()
    {
        _service.Setup(s => s.GetItems(It.IsAny<string?>(), It.IsAny<ItemSortField>(), It.IsAny<bool>()))
            .Returns(Ok());
        var vm = Create(RoleNames.AuctionManager);
        vm.SearchText = "чай";

        await vm.ClearSearchCommand.ExecuteAsync();

        Assert.Equal(string.Empty, vm.SearchText);
        _service.Verify(s => s.GetItems("", It.IsAny<ItemSortField>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task ServiceError_IsShown()
    {
        _service.Setup(s => s.GetItems(It.IsAny<string?>(), It.IsAny<ItemSortField>(), It.IsAny<bool>()))
            .Returns(OperationResult<IReadOnlyList<ItemDto>>.Fail("Немає доступу."));
        var vm = Create(RoleNames.Viewer);

        await vm.LoadAsync();

        Assert.Equal("Немає доступу.", vm.ErrorMessage);
    }

    [Fact]
    public async Task CreateAuction_Manager_ActiveItemSelected_NavigatesToEditor()
    {
        _service.Setup(s => s.GetItems(It.IsAny<string?>(), It.IsAny<ItemSortField>(), It.IsAny<bool>()))
            .Returns(Ok(new ItemDto { Id = 1, Name = "А", IsActive = true }));
        var vm = Create(RoleNames.AuctionManager);
        await vm.LoadAsync();
        Assert.False(vm.CreateAuctionCommand.CanExecute(null)); // нічого не вибрано

        vm.SelectedItem = vm.Items[0];
        Assert.True(vm.CreateAuctionCommand.CanExecute(null));
        vm.CreateAuctionCommand.Execute(null);

        Assert.Equal(typeof(AuctionEditViewModel), _navigation.LastTarget);
    }

    [Fact]
    public async Task CreateAuction_InactiveItem_IsNotAvailable()
    {
        _service.Setup(s => s.GetItems(It.IsAny<string?>(), It.IsAny<ItemSortField>(), It.IsAny<bool>()))
            .Returns(Ok(new ItemDto { Id = 1, IsActive = false }));
        var vm = Create(RoleNames.AuctionManager);
        await vm.LoadAsync();

        vm.SelectedItem = vm.Items[0];

        Assert.False(vm.CreateAuctionCommand.CanExecute(null));
    }

    [Fact]
    public async Task CreateAuction_Viewer_IsNotAvailable()
    {
        _service.Setup(s => s.GetItems(It.IsAny<string?>(), It.IsAny<ItemSortField>(), It.IsAny<bool>()))
            .Returns(Ok(new ItemDto { Id = 1, IsActive = true }));
        var vm = Create(RoleNames.Viewer);
        await vm.LoadAsync();

        vm.SelectedItem = vm.Items[0];

        Assert.False(vm.CanManageAuctions);
        Assert.False(vm.CreateAuctionCommand.CanExecute(null));
    }
}
