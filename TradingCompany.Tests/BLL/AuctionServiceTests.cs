using Moq;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.BLL.Validation;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.Tests.BLL;

public class AuctionServiceTests
{
    private readonly Mock<IAuctionDal> _auctions = new();
    private readonly Mock<ICurrencyDal> _currencies = new();
    private readonly Mock<IItemDal> _items = new();

    private AuctionService CreateService(string? role) =>
        new(_auctions.Object, _currencies.Object, _items.Object, new AuctionValidator(),
            TestUsers.Session(role), new PermissionService());

    private static AuctionDto ValidAuction() => new()
    {
        ItemId = 1,
        CurrencyId = 1,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(5),
        StartPrice = 100,
        BuyoutPrice = 300
    };

    [Fact]
    public void GetActive_Viewer_ReturnsAuctions()
    {
        _auctions.Setup(a => a.GetActive()).Returns(new List<AuctionDto> { new() { Id = 1 } });
        var result = CreateService(RoleNames.Viewer).GetActive();
        Assert.True(result.Success);
        Assert.Single(result.Value!);
    }

    [Fact]
    public void GetFinished_WithoutRole_Fails()
    {
        var result = CreateService(null).GetFinished();
        Assert.False(result.Success);
        _auctions.Verify(a => a.GetFinished(), Times.Never);
    }

    [Fact]
    public void Create_Viewer_IsForbidden()
    {
        var result = CreateService(RoleNames.Viewer).Create(ValidAuction());
        Assert.False(result.Success);
        _auctions.Verify(a => a.Add(It.IsAny<AuctionDto>()), Times.Never);
    }

    [Fact]
    public void Create_InvalidData_IsRejectedBeforeSaving()
    {
        var auction = ValidAuction();
        auction.StartPrice = 0;

        var result = CreateService(RoleNames.AuctionManager).Create(auction);

        Assert.False(result.Success);
        _auctions.Verify(a => a.Add(It.IsAny<AuctionDto>()), Times.Never);
    }

    [Fact]
    public void Create_ItemNotFound_Fails()
    {
        _items.Setup(i => i.GetById(1)).Returns((ItemDto?)null);
        var result = CreateService(RoleNames.AuctionManager).Create(ValidAuction());
        Assert.False(result.Success);
        _auctions.Verify(a => a.Add(It.IsAny<AuctionDto>()), Times.Never);
    }

    [Fact]
    public void Create_InactiveItem_Fails()
    {
        _items.Setup(i => i.GetById(1)).Returns(new ItemDto { Id = 1, IsActive = false });
        var result = CreateService(RoleNames.AuctionManager).Create(ValidAuction());
        Assert.False(result.Success);
    }

    [Fact]
    public void Create_Valid_SavesActiveAuction()
    {
        _items.Setup(i => i.GetById(1)).Returns(new ItemDto { Id = 1, IsActive = true });
        var auction = ValidAuction();
        auction.IsActive = false;

        var result = CreateService(RoleNames.AuctionManager).Create(auction);

        Assert.True(result.Success);
        _auctions.Verify(a => a.Add(It.Is<AuctionDto>(x => x.IsActive && x.ItemId == 1)), Times.Once);
    }

    [Fact]
    public void Deactivate_Viewer_IsForbidden()
    {
        var result = CreateService(RoleNames.Viewer).Deactivate(1);
        Assert.False(result.Success);
        _auctions.Verify(a => a.Deactivate(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Deactivate_NotFound_Fails()
    {
        _auctions.Setup(a => a.GetById(5)).Returns((AuctionDto?)null);
        Assert.False(CreateService(RoleNames.AuctionManager).Deactivate(5).Success);
    }

    [Fact]
    public void Deactivate_AlreadyInactive_Fails()
    {
        _auctions.Setup(a => a.GetById(5)).Returns(new AuctionDto { Id = 5, IsActive = false });
        Assert.False(CreateService(RoleNames.AuctionManager).Deactivate(5).Success);
        _auctions.Verify(a => a.Deactivate(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Deactivate_ActiveAuction_CallsDal()
    {
        _auctions.Setup(a => a.GetById(5)).Returns(new AuctionDto { Id = 5, IsActive = true });
        Assert.True(CreateService(RoleNames.AuctionManager).Deactivate(5).Success);
        _auctions.Verify(a => a.Deactivate(5), Times.Once);
    }
}
