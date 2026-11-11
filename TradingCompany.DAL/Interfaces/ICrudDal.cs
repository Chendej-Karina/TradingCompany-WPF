using TradingCompany.DAL.DTO;

namespace TradingCompany.DAL.Interfaces;


public interface ICrudDal<TDto> where TDto : BaseDto
{
    IReadOnlyList<TDto> GetAll();
    TDto? GetById(int id);
    
    void Add(TDto dto);
    void Update(TDto dto);
    void Delete(int id);
}
