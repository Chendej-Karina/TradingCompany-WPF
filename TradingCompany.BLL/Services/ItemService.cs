using TradingCompany.BLL.Common;
using TradingCompany.BLL.Security;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.BLL.Services;

public enum ItemSortField
{
    Id,
    Name,
    Category
}

public interface IItemService
{
    OperationResult<IReadOnlyList<ItemDto>> GetItems(string? search, ItemSortField sortBy, bool descending);
}

public class ItemService : IItemService
{
    private readonly IItemDal _items;
    private readonly IUserSession _session;
    private readonly IPermissionService _permissions;

    public ItemService(IItemDal items, IUserSession session, IPermissionService permissions)
    {
        _items = items;
        _session = session;
        _permissions = permissions;
    }

    public OperationResult<IReadOnlyList<ItemDto>> GetItems(string? search, ItemSortField sortBy, bool descending)
    {
        if (!_permissions.HasPermission(_session.Role, Permission.ViewItems))
            return OperationResult<IReadOnlyList<ItemDto>>.Fail("Немає доступу до перегляду товарів.");

        IEnumerable<ItemDto> items = string.IsNullOrWhiteSpace(search)
            ? _items.GetAll()
            : _items.Search(search.Trim());

        items = sortBy switch
        {
            ItemSortField.Name => descending ? items.OrderByDescending(i => i.Name) : items.OrderBy(i => i.Name),
            ItemSortField.Category => descending ? items.OrderByDescending(i => i.CategoryName) : items.OrderBy(i => i.CategoryName),
            _ => descending ? items.OrderByDescending(i => i.Id) : items.OrderBy(i => i.Id)
        };

        return OperationResult<IReadOnlyList<ItemDto>>.Ok(items.ToList());
    }
}
