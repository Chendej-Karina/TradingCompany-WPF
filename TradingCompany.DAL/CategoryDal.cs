using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;
using TradingCompany.DAL.Models;

namespace TradingCompany.DAL;

public class CategoryDal : DalBase<Category, CategoryDto>, ICategoryDal
{
    public CategoryDal(TradingCompanyContext context) : base(context) { }

    protected override int GetId(Category e) => e.Id;
    protected override CategoryDto ToDto(Category e) => new() { Id = e.Id, Name = e.Name };
    protected override void Apply(CategoryDto d, Category e) => e.Name = d.Name;
}
