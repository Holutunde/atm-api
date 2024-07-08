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

        public AtmTransactionController(IMediator mediator, JwtTokenService jwtTokenService, GetEmailService getEmailService)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
            _getEmailService = getEmailService;
        }

        [HttpPost("access")]
        public async Task<IActionResult> Access([FromBody] AtmAccessCommand command)
        {
            try
            {
                var (Token, ErrorMessage) = await _mediator.Send(command);

                if (ErrorMessage != null)
                {
                    return BadRequest(new { error = ErrorMessage });
                }

                return Ok(new { token = Token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand depositDto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
