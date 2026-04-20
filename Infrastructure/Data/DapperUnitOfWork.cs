using System;
using System.Data;
using System.Threading.Tasks;
using practice.Application.Interfaces;

namespace practice.Infrastructure.Data;

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private bool _disposed;

    public DapperUnitOfWork(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
        
        _transaction = _connection.BeginTransaction();
    }

    public IDbConnection Connection => _connection;
    public IDbTransaction Transaction => _transaction;

    public Task<bool> SaveChangesAsync()
    {
        try
        {
            _transaction.Commit();
            return Task.FromResult(true);
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = _connection.BeginTransaction();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }
}
