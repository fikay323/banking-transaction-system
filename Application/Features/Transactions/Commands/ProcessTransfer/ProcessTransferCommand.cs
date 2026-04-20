using MediatR;
using practice.Application.DTOs;

namespace practice.Application.Features.Transactions.Commands.ProcessTransfer;

public record ProcessTransferCommand(TransferRequestDto Request) : IRequest<bool>;
