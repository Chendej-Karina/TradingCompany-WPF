using Microsoft.EntityFrameworkCore;
using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class ItemDal : DalBase<Item, ItemDto>, IItemDal
{
    public ItemDal(TradingCompanyContext context) : base(context) { }

    protected override IQueryable<Item> Query =>
        _set.Include(i => i.Category).Include(i => i.Condition);

    protected override int GetId(Item e) => e.Id;

    protected override ItemDto ToDto(Item e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        CategoryId = e.CategoryId,
        ConditionId = e.ConditionId,
        IsActive = e.IsActive,
        CategoryName = e.Category?.Name ?? string.Empty,
        ConditionName = e.Condition?.Name ?? string.Empty
    };

    protected override void Apply(ItemDto d, Item e)
    {
        e.Name = d.Name;
        e.Description = d.Description;
        e.CategoryId = d.CategoryId;
        e.ConditionId = d.ConditionId;
        e.IsActive = d.IsActive;
    }

    public IReadOnlyList<ItemDto> Search(string text) =>
        Map(Query.Where(i => i.Name.Contains(text) ||
                             (i.Description != null && i.Description.Contains(text))));

    public IReadOnlyList<ItemDto> GetSortedByName(bool descending = false) =>
        Map(descending ? Query.OrderByDescending(i => i.Name) : Query.OrderBy(i => i.Name));

    public void Deactivate(int id)
    {
        var item = _set.Find(id);
        if (item == null) return;
        item.IsActive = false;
        _context.SaveChanges();
    }
}
