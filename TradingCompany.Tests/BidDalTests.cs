using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class BidDalTests : TestBase
{
    private BidDal Dal => new(Context);

    private BidDto NewBid() => new()
    {
        AuctionId = Context.Auctions.First().Id,
        Amount = 777,
        BidDate = DateTime.Now
    };

    [Fact]
    public void GetAll_ReturnsData() => Assert.NotEmpty(Dal.GetAll());

    [Fact]
    public void Add_And_GetById_Works()
    {
        var b = NewBid();
        Dal.Add(b);
        Assert.NotNull(Dal.GetById(b.Id));
    }

    [Fact]
    public void Update_ChangesAmount()
    {
        var b = NewBid();
        Dal.Add(b);
        b.Amount = 888;
        Dal.Update(b);
        Assert.Equal(888m, Dal.GetById(b.Id)!.Amount);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var b = NewBid();
        Dal.Add(b);
        Dal.Delete(b.Id);
        Assert.Null(Dal.GetById(b.Id));
    }

    [Fact]
    public void GetByAuction_ReturnsOnlyBidsOfThatAuction()
    {
        var b = NewBid();
        Dal.Add(b);
        var bids = Dal.GetByAuction(b.AuctionId);
        Assert.Contains(bids, x => x.Id == b.Id);
        Assert.All(bids, x => Assert.Equal(b.AuctionId, x.AuctionId));
    }
}
