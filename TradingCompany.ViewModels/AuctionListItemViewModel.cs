using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;

public class AuctionListItemViewModel : ObservableObject
{
    public AuctionListItemViewModel(AuctionDto auction) => Auction = auction;

    public AuctionDto Auction { get; }

    public int Id => Auction.Id;
    public string ItemName => Auction.ItemName;
    public DateTime StartDate => Auction.StartDate;
    public DateTime EndDate => Auction.EndDate;
    public string StartPriceText => $"{Auction.StartPrice:N2} {Auction.CurrencyCode}";
    public string BuyoutPriceText => Auction.BuyoutPrice.HasValue
        ? $"{Auction.BuyoutPrice.Value:N2} {Auction.CurrencyCode}"
        : "—";

    
    public bool IsOpen => Auction.IsActive && Auction.EndDate > DateTime.Now;

    public string StatusText =>
        !Auction.IsActive ? "Деактивований" :
        Auction.EndDate <= DateTime.Now ? "Завершений" : "Активний";
}
