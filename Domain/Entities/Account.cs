using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Account
{
    public int Id { get; private set; }
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public Customer Customer { get; private set; }
    public RewardState RewardState { get; private set; }
    
    private readonly List<Transaction> _transactions;
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    public Account(int id, string accountNumber, decimal initialBalance, Customer customer, RewardState rewardState)
    {
        Id = id;
        AccountNumber = accountNumber;
        Balance = initialBalance;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        RewardState = rewardState ?? throw new ArgumentNullException(nameof(rewardState));
        _transactions = new List<Transaction>();
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        Balance += amount;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        if (Balance < amount) throw new InvalidOperationException("Insufficient funds.");
        Balance -= amount;
    }

    public void ProcessTransaction(Transaction transaction, DateTime currentDate)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        if (transaction.AccountNumber != AccountNumber)
            throw new ArgumentException("Transaction does not belong to this account", nameof(transaction));

        // 1. Add to transaction history
        _transactions.Add(transaction);

        // 2. Evaluate Reward Rules
        
        // Rule: Airtime exclusion
        if (transaction.Type == TransactionType.Airtime)
        {
            return; // No points earned for Airtime
        }

        // Rule: Base points logic (1 point per whole amount unit)
        int basePoints = (int)Math.Floor(transaction.Amount);
        if (basePoints <= 0) return;

        // Rule: Corporate vs Individual points calculation
        int multiplier = Customer.Type == CustomerType.Corporate ? 2 : 1;
        
        double calculatedPoints = basePoints * multiplier;

        // Rule: Tenure check > 4 years
        if (Customer.HasTenureGreaterThan(4, currentDate))
        {
            // Bonus for long tenure, e.g. 1.5x points
            calculatedPoints *= 1.5;
        }

        // Add computed points to state
        int finalPoints = (int)Math.Round(calculatedPoints, MidpointRounding.AwayFromZero);
        if (finalPoints > 0)
        {
            RewardState.AddPoints(finalPoints);
        }
    }
}
