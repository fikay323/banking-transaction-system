using Domain.Entities;

namespace practice.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Account?> GetAccountByNumberAsync(string accountNumber);
    Task UpdateAccountAsync(Account account);
    Task AddTransactionAsync(Transaction transaction);
}
