using Microsoft.EntityFrameworkCore;
using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class AuctionDal : DalBase<Auction, AuctionDto>, IAuctionDal
{
    public AuctionDal(TradingCompanyContext context) : base(context) { }

    protected override IQueryable<Auction> Query =>
        _set.Include(a => a.Item).Include(a => a.Currency);

    protected override int GetId(Auction e) => e.Id;

    protected override AuctionDto ToDto(Auction e) => new()
    {
        Id = e.Id,
        ItemId = e.ItemId,
        CurrencyId = e.CurrencyId,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        StartPrice = e.StartPrice,
        BuyoutPrice = e.BuyoutPrice,
        IsActive = e.IsActive,
        ItemName = e.Item?.Name ?? string.Empty,
        CurrencyCode = e.Currency?.Code ?? string.Empty
    };

    protected override void Apply(AuctionDto d, Auction e)
    {
        e.ItemId = d.ItemId;
        e.CurrencyId = d.CurrencyId;
        e.StartDate = d.StartDate;
        e.EndDate = d.EndDate;
        e.StartPrice = d.StartPrice;
        e.BuyoutPrice = d.BuyoutPrice;
        e.IsActive = d.IsActive;
    }

    
    public IReadOnlyList<AuctionDto> GetActive()
    {
        var now = DateTime.Now;
        return Map(Query.Where(a => a.IsActive && a.EndDate > now).OrderBy(a => a.EndDate));
    }

    
    public IReadOnlyList<AuctionDto> GetFinished()
    {
        var now = DateTime.Now;
        return Map(Query.Where(a => !a.IsActive || a.EndDate <= now).OrderByDescending(a => a.EndDate));
    }

    public IReadOnlyList<AuctionDto> GetByItem(int itemId) =>
        Map(Query.Where(a => a.ItemId == itemId));

    public void Deactivate(int id)
    {
        var auction = _set.Find(id);
        if (auction == null) return;
        auction.IsActive = false;
        _context.SaveChanges();
    }
}
