using practice.Application.Interfaces;

namespace practice.API.Services
{
    public class ConsoleAuditService : IAuditService
    {
        public Task LogActivityAsync(string action, string accountNumber, string details)
        {
            Console.WriteLine($"[AUDIT] Action: {action}, Account: {accountNumber}, Details: {details}");
            return Task.CompletedTask;
        }
    }
}
