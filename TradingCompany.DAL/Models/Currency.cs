using System;
using System.Collections.Generic;

namespace TradingCompany.DAL.Models;

public partial class Currency
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();
}
