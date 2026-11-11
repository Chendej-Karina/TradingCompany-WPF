using TradingCompany.DAL;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests;

public class CurrencyDalTests : TestBase
{
    private CurrencyDal Dal => new(Context);

    [Fact]
    public void GetAll_ReturnsData() => Assert.NotEmpty(Dal.GetAll());

    [Fact]
    public void Add_And_GetById_Works()
    {
        var c = new CurrencyDto { Code = "TST", Name = "Тестова валюта" };
        Dal.Add(c);
        var found = Dal.GetById(c.Id)!;
        Assert.Equal("TST", found.Code);
        Assert.Equal("Тестова валюта", found.Name);
    }

    [Fact]
    public void Update_ChangesName()
    {
        var c = new CurrencyDto { Code = "TS2", Name = "Стара" };
        Dal.Add(c);
        c.Name = "Нова";
        Dal.Update(c);
        Assert.Equal("Нова", Dal.GetById(c.Id)!.Name);
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var c = new CurrencyDto { Code = "TS3", Name = "Видалити" };
        Dal.Add(c);
        Dal.Delete(c.Id);
        Assert.Null(Dal.GetById(c.Id));
    }
}
