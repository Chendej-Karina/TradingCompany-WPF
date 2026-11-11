using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TradingCompany.DAL.Data;

namespace TradingCompany.Tests;


public abstract class TestBase : IDisposable
{
    protected readonly TradingCompanyContext Context;
    private readonly IDbContextTransaction _transaction;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<TradingCompanyContext>()
            .UseSqlServer(@"Server=DESKTOP-GC3UDDI\SQLEXPRESS;Database=TradingCompany;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        Context = new TradingCompanyContext(options);
        _transaction = Context.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction.Rollback();
        _transaction.Dispose();
        Context.Dispose();
    }
}
