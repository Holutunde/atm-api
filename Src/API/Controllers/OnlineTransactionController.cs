using Application.Dto;
using Application.Interfaces;
using Application.Online.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class OnlineTransactionController(
       IMediator mediator, IGetEmailService getEmailService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private readonly IGetEmailService _getEmailService = getEmailService;

        private string GetCurrentEmail()
        {
            return _getEmailService.GetEmailFromToken(User);
        }


        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
            try
            {
                var email = GetCurrentEmail();

                var command = new CheckBalanceOnlineCommand { Email = email };
                var owner  = await _mediator.Send(command);

                return Ok(new { owner });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto depositDto)
        {
            try
            {
                var email = GetCurrentEmail();
                var command = new DepositOnlineCommand { Email = email, Amount = depositDto.Amount };
                var balance = await _mediator.Send(command);

                return Ok(balance);
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
                var email = GetCurrentEmail();
                var command = new TransferOnlineCommand { SenderEmail = email, ReceiverAccountNumber = transferDto.ReceiverAccountNumber, Amount = transferDto.Amount };
                var senderBalance= await _mediator.Send(command);

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
                var email = GetCurrentEmail();
                var command = new ChangePinOnlineCommand { Email = email, NewPin = changePinDto.NewPin };
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
