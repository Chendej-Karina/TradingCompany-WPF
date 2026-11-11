using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class CurrencyDal : DalBase<Currency, CurrencyDto>, ICurrencyDal
{
    public CurrencyDal(TradingCompanyContext context) : base(context) { }

    protected override int GetId(Currency e) => e.Id;
    protected override CurrencyDto ToDto(Currency e) => new() { Id = e.Id, Code = e.Code, Name = e.Name };

    protected override void Apply(CurrencyDto d, Currency e)
    {
        e.Code = d.Code;
        e.Name = d.Name;
    }
}
