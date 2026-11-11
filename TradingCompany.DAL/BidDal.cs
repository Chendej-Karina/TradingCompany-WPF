using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class BidDal : DalBase<Bid, BidDto>, IBidDal
{
    public BidDal(TradingCompanyContext context) : base(context) { }

    protected override int GetId(Bid e) => e.Id;

    protected override BidDto ToDto(Bid e) => new()
    {
        Id = e.Id,
        AuctionId = e.AuctionId,
        Amount = e.Amount,
        BidDate = e.BidDate
    };

    protected override void Apply(BidDto d, Bid e)
    {
        e.AuctionId = d.AuctionId;
        e.Amount = d.Amount;
        e.BidDate = d.BidDate;
    }

    public IReadOnlyList<BidDto> GetByAuction(int auctionId) =>
        Map(_set.Where(b => b.AuctionId == auctionId).OrderBy(b => b.BidDate));
}
