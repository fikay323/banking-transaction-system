namespace practice.Application.Interfaces;

public interface IAuditService
{
    Task LogActivityAsync(string action, string accountNumber, string details);
}
