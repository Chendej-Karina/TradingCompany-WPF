using TradingCompany.DAL.DTO;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;


public class ItemRowViewModel : ObservableObject
{
    public ItemRowViewModel(ItemDto item) => Item = item;

    public ItemDto Item { get; }

    public int Id => Item.Id;
    public string Name => Item.Name;
    public string Description => Item.Description ?? string.Empty;
    public string CategoryName => Item.CategoryName;
    public string ConditionName => Item.ConditionName;
    public bool IsActive => Item.IsActive;
    public string StatusText => Item.IsActive ? "Активний" : "Деактивований";
}
