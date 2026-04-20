using System;

namespace Domain.Entities;

public class Transaction
{
    public int Id { get; private set; }
    public string AccountNumber { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Transaction(int id, string accountNumber, decimal amount, TransactionType type, DateTime timestamp)
    {
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be strictly greater than zero", nameof(amount));

        Id = id;
        AccountNumber = accountNumber;
        Amount = amount;
        Type = type;
        Timestamp = timestamp;
    }
}
