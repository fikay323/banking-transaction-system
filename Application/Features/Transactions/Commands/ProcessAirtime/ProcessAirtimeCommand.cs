using MediatR;
using practice.Application.DTOs;

namespace practice.Application.Features.Transactions.Commands.ProcessAirtime;

public class ProcessAirtimeCommand(AirtimeRequestDto request) : IRequest<bool>
{
    public AirtimeRequestDto Request { get; set; } = request;
}
