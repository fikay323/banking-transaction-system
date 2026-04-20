using Domain.Entities;
using MediatR;
using practice.Application.Interfaces;

namespace practice.Application.Features.Transactions.Commands.ProcessTransfer;

public class ProcessTransferCommandHandler(
    ITransactionRepository repository,
    IUnitOfWork unitOfWork,
    IAuditService auditService)
    : IRequestHandler<ProcessTransferCommand, bool>
{
    public async Task<bool> Handle(ProcessTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Request;

        // 1. Validate & Fetch State
        var sourceAccount = await repository.GetAccountByNumberAsync(dto.SourceAccountNumber);
        if (sourceAccount == null) throw new InvalidOperationException("Source account not found.");

        var destinationAccount = await repository.GetAccountByNumberAsync(dto.DestinationAccountNumber);
        if (destinationAccount == null) throw new InvalidOperationException("Destination account not found.");

        // 2. Perform actions inside Domain (Debit & Credit orchestrate Validations)
        sourceAccount.Debit(dto.Amount);
        destinationAccount.Credit(dto.Amount);

        // 3. Setup Transactions 
        var sourceTx = new Transaction(0, sourceAccount.AccountNumber, dto.Amount, TransactionType.Transfer, DateTime.UtcNow);
        var destTx = new Transaction(0, destinationAccount.AccountNumber, dto.Amount, TransactionType.Transfer, DateTime.UtcNow);

        // 4. Trigger Domain rules (Point calculations etc)
        sourceAccount.ProcessTransaction(sourceTx, DateTime.UtcNow);
        destinationAccount.ProcessTransaction(destTx, DateTime.UtcNow);

        // 5. Persist modifications
        await repository.UpdateAccountAsync(sourceAccount);
        await repository.UpdateAccountAsync(destinationAccount);
        await repository.AddTransactionAsync(sourceTx);
        await repository.AddTransactionAsync(destTx);

        await auditService.LogActivityAsync("Transfer", dto.SourceAccountNumber, $"Transferred {dto.Amount} to {dto.DestinationAccountNumber}");

        // 6. Commit to database via Dapper Unit Of Work
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}
