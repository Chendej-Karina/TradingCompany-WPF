using System;
using System.Collections.Generic;

namespace TradingCompany.DAL.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int ConditionId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();

    public virtual Category Category { get; set; } = null!;

    public virtual Condition Condition { get; set; } = null!;
}
