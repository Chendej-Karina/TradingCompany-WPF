using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class ConditionDal : DalBase<Condition, ConditionDto>, IConditionDal
{
    public ConditionDal(TradingCompanyContext context) : base(context) { }

    protected override int GetId(Condition e) => e.Id;
    protected override ConditionDto ToDto(Condition e) => new() { Id = e.Id, Name = e.Name };
    protected override void Apply(ConditionDto d, Condition e) => e.Name = d.Name;
}
