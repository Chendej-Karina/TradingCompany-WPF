using System;
using System.Collections.Generic;

namespace TradingCompany.DAL.Models;

public partial class Auction
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int CurrencyId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal StartPrice { get; set; }

    public decimal? BuyoutPrice { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual Currency Currency { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
