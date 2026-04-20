using System.Data;

namespace practice.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    Task<bool> SaveChangesAsync();
}
