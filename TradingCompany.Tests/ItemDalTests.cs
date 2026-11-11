using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class ItemDalTests : TestBase
{
    private ItemDal Dal => new(Context);

    private ItemDto NewItem(string name) => new()
    {
        Name = name,
        Description = "Опис",
        CategoryId = Context.Categories.First().Id,
        ConditionId = Context.Conditions.First().Id,
        IsActive = true
    };

    [Fact]
    public void GetAll_ReturnsData_WithCategoryAndConditionNames()
    {
        var first = Dal.GetAll().First();
        Assert.False(string.IsNullOrEmpty(first.CategoryName));
        Assert.False(string.IsNullOrEmpty(first.ConditionName));
    }

    [Fact]
    public void Add_And_GetById_Works()
    {
        var i = NewItem("Тестовий товар");
        Dal.Add(i);
        Assert.True(i.Id > 0);
        Assert.Equal("Тестовий товар", Dal.GetById(i.Id)!.Name);
    }

    [Fact]
    public void Update_ChangesName()
    {
        var i = NewItem("Старий");
        Dal.Add(i);
        i.Name = "Новий";
        Dal.Update(i);
        Assert.Equal("Новий", Dal.GetById(i.Id)!.Name);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var i = NewItem("Видалити");
        Dal.Add(i);
        Dal.Delete(i.Id);
        Assert.Null(Dal.GetById(i.Id));
    }

    [Fact]
    public void Search_FindsByName()
    {
        Dal.Add(NewItem("УнікальнийТовар123"));
        Assert.Single(Dal.Search("УнікальнийТовар123"));
    }

    [Fact]
    public void Search_FindsByDescription()
    {
        var i = NewItem("Звичайний");
        i.Description = "УнікальнийОпис456";
        Dal.Add(i);
        Assert.Single(Dal.Search("УнікальнийОпис456"));
    }

    [Fact]
    public void Search_NothingFound_ReturnsEmpty() =>
        Assert.Empty(Dal.Search("такого-немає-xyz-999"));

    [Fact]
    public void GetSortedByName_IsSortedAscending()
    {
        var names = Dal.GetSortedByName().Select(i => i.Name).ToList();
        var expected = names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        Assert.Equal(expected, names);
    }

    [Fact]
    public void GetSortedByName_Descending_IsReversed()
    {
        var asc = Dal.GetSortedByName().Select(i => i.Name).ToList();
        var desc = Dal.GetSortedByName(true).Select(i => i.Name).ToList();
        asc.Reverse();
        Assert.Equal(asc, desc);
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var i = NewItem("Деактивувати");
        Dal.Add(i);
        Dal.Deactivate(i.Id);
        Assert.False(Dal.GetById(i.Id)!.IsActive);
    }
}
