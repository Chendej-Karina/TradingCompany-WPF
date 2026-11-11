using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class AuctionDalTests : TestBase
{
    private AuctionDal Dal => new(Context);

    private AuctionDto NewAuction() => new()
    {
        ItemId = Context.Items.First().Id,
        CurrencyId = Context.Currencies.First().Id,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(5),
        StartPrice = 100,
        BuyoutPrice = 500,
        IsActive = true
    };

    [Fact]
    public void GetAll_ReturnsData_WithItemNameAndCurrencyCode()
    {
        var first = Dal.GetAll().First();
        Assert.False(string.IsNullOrEmpty(first.ItemName));
        Assert.False(string.IsNullOrEmpty(first.CurrencyCode));
    }

    [Fact]
    public void Add_And_GetById_Works()
    {
        var a = NewAuction();
        Dal.Add(a);
        Assert.True(a.Id > 0);
        Assert.NotNull(Dal.GetById(a.Id));
    }

    [Fact]
    public void Update_ChangesStartPrice()
    {
        var a = NewAuction();
        Dal.Add(a);
        a.StartPrice = 999;
        Dal.Update(a);
        Assert.Equal(999m, Dal.GetById(a.Id)!.StartPrice);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var a = NewAuction();
        Dal.Add(a);
        Dal.Delete(a.Id);
        Assert.Null(Dal.GetById(a.Id));
    }

    [Fact]
    public void GetActive_ContainsNewActiveAuction()
    {
        var a = NewAuction();
        Dal.Add(a);
        Assert.Contains(Dal.GetActive(), x => x.Id == a.Id);
    }

    [Fact]
    public void GetFinished_ContainsAuctionWhoseTimeIsUp()
    {
        var a = NewAuction();
        a.StartDate = DateTime.Now.AddDays(-10);
        a.EndDate = DateTime.Now.AddDays(-1);
        Dal.Add(a);
        Assert.Contains(Dal.GetFinished(), x => x.Id == a.Id);
        Assert.DoesNotContain(Dal.GetActive(), x => x.Id == a.Id);
    }

    [Fact]
    public void Deactivate_MovesAuctionToFinished()
    {
        var a = NewAuction();
        Dal.Add(a);
        Dal.Deactivate(a.Id);
        Assert.Contains(Dal.GetFinished(), x => x.Id == a.Id);
        Assert.DoesNotContain(Dal.GetActive(), x => x.Id == a.Id);
    }

    [Fact]
    public void GetByItem_ReturnsAuctionsOfItem()
    {
        var a = NewAuction();
        Dal.Add(a);
        Assert.Contains(Dal.GetByItem(a.ItemId), x => x.Id == a.Id);
    }
}
