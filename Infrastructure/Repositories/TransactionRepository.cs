using Dapper;
using Domain.Entities;
using practice.Application.Interfaces;

namespace practice.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
    {
        var sql = @"
            SELECT 
                a.AccountId, a.AccountNumber, a.AccountBalance,
                c.CustomerId, c.CustomerName, c.CustomerType, c.DateCreated,
                ISNULL(r.TotalPoints, 0) as TotalPoints
            FROM [dbo].[AccountData] a
            INNER JOIN [dbo].[CustomerData] c ON a.CustomerId = c.CustomerId
            LEFT JOIN [dbo].[CustomerRewardState] r ON c.CustomerId = r.CustomerId AND r.RewardMonth = @RewardMonth
            WHERE a.AccountNumber = @AccountNumber";

        var rewardMonth = DateTime.UtcNow.ToString("yyyy-MM");

        var row = await _unitOfWork.Connection.QuerySingleOrDefaultAsync(
            sql,
            new { AccountNumber = accountNumber, RewardMonth = rewardMonth },
            transaction: _unitOfWork.Transaction);

        if (row == null) return null;

        var customerType = row.CustomerType == "BUSINESS" ? CustomerType.Corporate : CustomerType.Individual;

        var customer = new Customer((long)row.CustomerId, (string)row.CustomerName, customerType,
            (DateTime)row.DateCreated);
        var rewardState = new RewardState((long)row.CustomerId, rewardMonth, (int)row.TotalPoints);

        var account = new Account((int)row.AccountId, (string)row.AccountNumber, (decimal)row.AccountBalance, customer,
            rewardState);

        return account;
    }

    public async Task UpdateAccountAsync(Account account)
    {
        // 1. Update the physical account balance
        var updateBalanceSql = @"
            UPDATE [dbo].[AccountData] 
            SET AccountBalance = @Balance 
            WHERE AccountNumber = @AccountNumber";

        await _unitOfWork.Connection.ExecuteAsync(
            updateBalanceSql,
            new { account.Balance, account.AccountNumber },
            transaction: _unitOfWork.Transaction);

        // 2. Upsert the Customer Reward State based on RewardMonth logic
        var upsertRewardSql = @"
            IF EXISTS (SELECT 1 FROM [dbo].[CustomerRewardState] WHERE CustomerId = @CustomerId AND RewardMonth = @RewardMonth)
            BEGIN
                UPDATE [dbo].[CustomerRewardState]
                SET TotalPoints = @TotalPoints, LastUpdated = GETUTCDATE()
                WHERE CustomerId = @CustomerId AND RewardMonth = @RewardMonth
            END
            ELSE
            BEGIN
                INSERT INTO [dbo].[CustomerRewardState] (CustomerId, RewardMonth, TotalPoints, LastUpdated)
                VALUES (@CustomerId, @RewardMonth, @TotalPoints, GETUTCDATE())
            END";

        await _unitOfWork.Connection.ExecuteAsync(
            upsertRewardSql,
            new
            {
                CustomerId = account.RewardState.CustomerId,
                RewardMonth = account.RewardState.RewardMonth,
                TotalPoints = account.RewardState.TotalPoints
            },
            transaction: _unitOfWork.Transaction);
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        // Inserting explicit records, DiscountedAmount & Rate deliberately hardcoded to 0.00
        var sql = @"
            INSERT INTO [dbo].[TransactionData] (AccountNumber, Amount, DiscountedAmount, Rate, TransactionDate)
            VALUES (@AccountNumber, @Amount, 0.00, 0.00, @TransactionDate)";

        await _unitOfWork.Connection.ExecuteAsync(
            sql,
            new
            {
                AccountNumber = transaction.AccountNumber,
                Amount = transaction.Amount,
                TransactionDate = transaction.Timestamp
            },
            transaction: _unitOfWork.Transaction);
    }
}
