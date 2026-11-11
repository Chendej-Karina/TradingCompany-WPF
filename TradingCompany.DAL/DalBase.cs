using Microsoft.EntityFrameworkCore;
using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.DAL;


public abstract class DalBase<TEntity, TDto> : ICrudDal<TDto>
    where TEntity : class, new()
    where TDto : BaseDto
{
    protected readonly TradingCompanyContext _context;
    protected readonly DbSet<TEntity> _set;

    protected DalBase(TradingCompanyContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }

    
    protected virtual IQueryable<TEntity> Query => _set;

    protected abstract TDto ToDto(TEntity entity);

    
    protected abstract void Apply(TDto dto, TEntity entity);

    protected abstract int GetId(TEntity entity);

    protected IReadOnlyList<TDto> Map(IQueryable<TEntity> query) =>
        query.AsNoTracking().ToList().Select(ToDto).ToList();

    public virtual IReadOnlyList<TDto> GetAll() => Map(Query);

    public virtual TDto? GetById(int id)
    {
        var entity = Query.AsNoTracking().FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
        return entity == null ? null : ToDto(entity);
    }

    public virtual void Add(TDto dto)
    {
        var entity = new TEntity();
        Apply(dto, entity);
        _set.Add(entity);
        _context.SaveChanges();
        dto.Id = GetId(entity);
    }

    public virtual void Update(TDto dto)
    {
        var entity = _set.Find(dto.Id);
        if (entity == null) return;
        Apply(dto, entity);
        _context.SaveChanges();
    }

    public virtual void Delete(int id)
    {
        var entity = _set.Find(id);
        if (entity == null) return;
        _set.Remove(entity);
        _context.SaveChanges();
    }
}
