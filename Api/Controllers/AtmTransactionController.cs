using Application.Atms.Commands;
using Application.Dto;
using Application.Interfaces;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/atm")]
    public class AtmTransactionController(IMediator mediator, IGetEmailService getEmailService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private readonly IGetEmailService _getEmailService = getEmailService;
      

        [HttpPost("access")]
        public async Task<IActionResult> Access([FromBody] AtmAccessCommand command)
        {
            try
            {
                var token = await _mediator.Send(command);

                return Ok( token );
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
                var balance = await _mediator.Send(command);

                return Ok(balance);
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
                var balance = await _mediator.Send(command);

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
                var senderBalance = await _mediator.Send(command);


                return Ok(senderBalance);
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

                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
