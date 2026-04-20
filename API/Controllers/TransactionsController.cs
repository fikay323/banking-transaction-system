using MediatR;
using Microsoft.AspNetCore.Mvc;
using practice.Application.DTOs;
using practice.Application.Features.Transactions.Commands.ProcessAirtime;
using practice.Application.Features.Transactions.Commands.ProcessTransfer;

namespace practice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request)
        {
            var command = new ProcessTransferCommand(request);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("airtime")]
        public async Task<IActionResult> Airtime([FromBody] AirtimeRequestDto request)
        {
            var command = new ProcessAirtimeCommand(request);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
