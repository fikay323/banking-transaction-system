using System;

namespace Domain.Entities;

public class Customer
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public CustomerType Type { get; private set; }
    public DateTime DateJoined { get; private set; }

    public Customer(long id, string name, CustomerType type, DateTime dateJoined)
    {
        Id = id;
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name cannot be empty", nameof(name)) : name;
        Type = type;
        DateJoined = dateJoined;
    }

    public bool HasTenureGreaterThan(int years, DateTime currentDate)
    {
        // Simple tenure check
        return DateJoined.AddYears(years) < currentDate;
    }
}
