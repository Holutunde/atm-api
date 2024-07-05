using Application.Atms.Commands;
using Application.Dto;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Api.Controllers
{
    [ApiController]
    [Route("api/atm")]
    public class AtmTransactionController : ControllerBase
    {

        private readonly IMediator _mediator;

        private readonly GetEmailService _getEmailService;

        private readonly JwtTokenService _jwtTokenService;


     public AtmTransactionController(IMediator mediator, JwtTokenService jwtTokenService, GetEmailService getEmailService
)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
             _getEmailService = getEmailService;
             
        }

        [HttpPost("access")]
        public async Task<IActionResult> Access([FromBody] AtmAccessCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.ErrorMessage != null)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new { token = result.Token });
        }


        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
            var accountNumber = _getEmailService.GetAccountNumberFromToken(User);
            var command = new CheckBalanceCommand { AccountNumber = accountNumber };
            var (balance, errorMessage) = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Unauthorized();
            }

            return Ok(new { balance });
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand depositDto)
        {
            var accountNumber = _getEmailService.GetAccountNumberFromToken(User);
            var command = new DepositCommand { AccountNumber = accountNumber, Amount = depositDto.Amount };
            var (balance, errorMessage) = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Unauthorized();
            }

            return Ok(new { balance });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            var accountNumber = _getEmailService.GetAccountNumberFromToken(User);
            var command = new TransferCommand { SenderAccountNumber = accountNumber, ReceiverAccountNumber = transferDto.ReceiverAccountNumber, Amount = transferDto.Amount };
            var (senderBalance, receiverBalance, errorMessage) = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (errorMessage == "Unauthorized" || errorMessage == "Insufficient balance.")
                {
                    return BadRequest(errorMessage);
                }
                return NotFound(errorMessage);
            }

            return Ok(new { senderBalance, receiverBalance });
        }

        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            var accountNumber = _getEmailService.GetAccountNumberFromToken(User);
            var command = new ChangePinCommand { AccountNumber = accountNumber, NewPin = changePinDto.NewPin };
            var errorMessage = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Unauthorized();
            }

            return NoContent();
        }
    }
}
