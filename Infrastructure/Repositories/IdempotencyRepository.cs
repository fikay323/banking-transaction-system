using System.Data;
using Dapper;
using practice.Application.Interfaces;

namespace practice.Infrastructure.Repositories;

public class IdempotencyRepository(IDbConnection connection) : IIdempotencyRepository
{
    public async Task<bool> EnsureIdempotencyAsync(Guid key, string requestName)
    {
        // Attempts an explicit insert to secure an idempotent registration lock
        var sql = @"
            INSERT INTO [dbo].[IdempotencyKeys] (IdempotencyKey, RequestName, CreatedAt)
            VALUES (@IdempotencyKey, @RequestName, GETUTCDATE());
        ";

        try
        {
            await connection.ExecuteAsync(sql, new { IdempotencyKey = key, RequestName = requestName });
            return true;
        }
        catch (Exception ex) when (ex.Message.Contains("Violation of PRIMARY KEY constraint"))
        {
            return false;
        }
    }
}
