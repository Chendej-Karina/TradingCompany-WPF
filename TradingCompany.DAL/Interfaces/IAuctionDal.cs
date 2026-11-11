using TradingCompany.DAL.DTO;

namespace TradingCompany.DAL.Interfaces;

public interface IAuctionDal : ICrudDal<AuctionDto>
{
    IReadOnlyList<AuctionDto> GetActive();
    IReadOnlyList<AuctionDto> GetFinished();
    IReadOnlyList<AuctionDto> GetByItem(int itemId);
    void Deactivate(int id);
}
