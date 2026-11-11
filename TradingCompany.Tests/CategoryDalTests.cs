using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class CategoryDalTests : TestBase
{
    private CategoryDal Dal => new(Context);

    [Fact]
    public void GetAll_ReturnsData() => Assert.NotEmpty(Dal.GetAll());

    [Fact]
    public void Add_And_GetById_Works()
    {
        var c = new CategoryDto { Name = "Тестова категорія" };
        Dal.Add(c);
        Assert.True(c.Id > 0);
        Assert.Equal("Тестова категорія", Dal.GetById(c.Id)!.Name);
    }

    [Fact]
    public void Update_ChangesName()
    {
        var c = new CategoryDto { Name = "Стара" };
        Dal.Add(c);
        c.Name = "Нова";
        Dal.Update(c);
        Assert.Equal("Нова", Dal.GetById(c.Id)!.Name);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var c = new CategoryDto { Name = "Видалити" };
        Dal.Add(c);
        Dal.Delete(c.Id);
        Assert.Null(Dal.GetById(c.Id));
    }
}
