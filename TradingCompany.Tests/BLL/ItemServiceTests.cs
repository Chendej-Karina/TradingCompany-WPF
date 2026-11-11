using Moq;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.Tests.BLL;

public class ItemServiceTests
{
    private readonly Mock<IItemDal> _items = new();

    private static List<ItemDto> Sample() => new()
    {
        new() { Id = 1, Name = "Чайник", CategoryName = "Дім" },
        new() { Id = 2, Name = "Автомобіль", CategoryName = "Транспорт" },
        new() { Id = 3, Name = "Бубон", CategoryName = "Музика" }
    };

    private ItemService CreateService(string? role) =>
        new(_items.Object, TestUsers.Session(role), new PermissionService());

    [Fact]
    public void GetItems_WithoutPermission_Fails()
    {
        var result = CreateService(null).GetItems(null, ItemSortField.Id, false);
        Assert.False(result.Success);
        _items.Verify(i => i.GetAll(), Times.Never);
    }

    [Fact]
    public void GetItems_NoSearch_ReturnsAllSortedById()
    {
        _items.Setup(i => i.GetAll()).Returns(Sample().OrderByDescending(i => i.Id).ToList());

        var result = CreateService(RoleNames.Viewer).GetItems("  ", ItemSortField.Id, false);

        Assert.True(result.Success);
        Assert.Equal(new[] { 1, 2, 3 }, result.Value!.Select(i => i.Id));
        _items.Verify(i => i.Search(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetItems_WithSearch_UsesSearchAndTrimsText()
    {
        _items.Setup(i => i.Search("чай")).Returns(new List<ItemDto> { Sample()[0] });

        var result = CreateService(RoleNames.AuctionManager).GetItems("  чай ", ItemSortField.Name, false);

        Assert.Single(result.Value!);
        _items.Verify(i => i.Search("чай"), Times.Once);
    }

    [Fact]
    public void GetItems_SortByNameDescending()
    {
        _items.Setup(i => i.GetAll()).Returns(Sample());

        var result = CreateService(RoleNames.Viewer).GetItems(null, ItemSortField.Name, true);

        Assert.Equal(new[] { "Чайник", "Бубон", "Автомобіль" }, result.Value!.Select(i => i.Name));
    }

    [Fact]
    public void GetItems_SortByCategory()
    {
        _items.Setup(i => i.GetAll()).Returns(Sample());

        var result = CreateService(RoleNames.Viewer).GetItems(null, ItemSortField.Category, false);

        Assert.Equal(new[] { "Дім", "Музика", "Транспорт" }, result.Value!.Select(i => i.CategoryName));
    }
}
