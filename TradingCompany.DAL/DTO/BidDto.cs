namespace TradingCompany.DAL.DTO;

public class BidDto : BaseDto
{
    public int AuctionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime BidDate { get; set; }
}
