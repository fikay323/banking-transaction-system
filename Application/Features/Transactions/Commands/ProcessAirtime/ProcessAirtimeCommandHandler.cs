using Domain.Entities;
using MediatR;
using practice.Application.Interfaces;

namespace practice.Application.Features.Transactions.Commands.ProcessAirtime;

public class ProcessAirtimeCommandHandler(
    ITransactionRepository repository,
    IUnitOfWork unitOfWork,
    IAuditService auditService)
    : IRequestHandler<ProcessAirtimeCommand, bool>
{
    public async Task<bool> Handle(ProcessAirtimeCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;

        var account = await repository.GetAccountByNumberAsync(dto.AccountNumber);
        if (account == null) throw new InvalidOperationException("Account not found.");

        // Deduct balance
        account.Debit(dto.Amount);

        // Generate Domain Transaction
        var tx = new Transaction(0, account.AccountNumber, dto.Amount, TransactionType.Airtime, DateTime.UtcNow);

        // Process rich domain rules (in this case, airtime yields no points)
        account.ProcessTransaction(tx, DateTime.UtcNow);

        // Persist
        await repository.UpdateAccountAsync(account);
        await repository.AddTransactionAsync(tx);

        await auditService.LogActivityAsync("Airtime Purchase", dto.AccountNumber, $"Purchased {dto.Amount} airtime for {dto.PhoneNumber}");

        // Commit transaction
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}
