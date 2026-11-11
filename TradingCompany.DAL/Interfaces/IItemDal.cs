using TradingCompany.DAL.DTO;

namespace TradingCompany.DAL.Interfaces;

public interface IItemDal : ICrudDal<ItemDto>
{
    IReadOnlyList<ItemDto> Search(string text);
    IReadOnlyList<ItemDto> GetSortedByName(bool descending = false);
    void Deactivate(int id);
}
