using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class ConditionDalTests : TestBase
{
    private ConditionDal Dal => new(Context);

    [Fact]
    public void GetAll_ReturnsData() => Assert.NotEmpty(Dal.GetAll());

    [Fact]
    public void Add_And_GetById_Works()
    {
        var c = new ConditionDto { Name = "Тестовий стан" };
        Dal.Add(c);
        Assert.Equal("Тестовий стан", Dal.GetById(c.Id)!.Name);
    }

    [Fact]
    public void Update_ChangesName()
    {
        var c = new ConditionDto { Name = "Стан A" };
        Dal.Add(c);
        c.Name = "Стан B";
        Dal.Update(c);
        Assert.Equal("Стан B", Dal.GetById(c.Id)!.Name);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var c = new ConditionDto { Name = "Видалити" };
        Dal.Add(c);
        Dal.Delete(c.Id);
        Assert.Null(Dal.GetById(c.Id));
    }
}
