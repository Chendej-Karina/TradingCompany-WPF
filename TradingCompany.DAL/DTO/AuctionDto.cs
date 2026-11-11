namespace TradingCompany.DAL.DTO;

public class AuctionDto : BaseDto
{
    public int ItemId { get; set; }
    public int CurrencyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal StartPrice { get; set; }
    public decimal? BuyoutPrice { get; set; }
    public bool IsActive { get; set; } = true;

    public string ItemName { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
}
