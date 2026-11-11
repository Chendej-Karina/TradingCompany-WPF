using System;
using System.Collections.Generic;

namespace TradingCompany.DAL.Models;

public partial class Bid
{
    public int Id { get; set; }

    public int AuctionId { get; set; }

    public decimal Amount { get; set; }

    public DateTime BidDate { get; set; }

    public virtual Auction Auction { get; set; } = null!;
}
