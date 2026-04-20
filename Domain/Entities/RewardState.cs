using System;

namespace Domain.Entities;

public class RewardState
{
    public long CustomerId { get; private set; }
    public string RewardMonth { get; private set; }
    public int TotalPoints { get; private set; }

    public RewardState(long customerId, string rewardMonth, int initialPoints = 0)
    {
        CustomerId = customerId;
        RewardMonth = rewardMonth;
        TotalPoints = initialPoints;
    }

    public void AddPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Cannot add negative points", nameof(points));
            
        TotalPoints += points;
    }
}
