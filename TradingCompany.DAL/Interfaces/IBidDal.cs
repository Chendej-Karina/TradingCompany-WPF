using TradingCompany.DAL.DTO;

namespace TradingCompany.DAL.Interfaces;

public interface IBidDal : ICrudDal<BidDto>
{
    IReadOnlyList<BidDto> GetByAuction(int auctionId);
}
