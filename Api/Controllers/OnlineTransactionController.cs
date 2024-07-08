using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Atms.Commands;
using Application.Dto;
using Application.Online.Commands;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Entities;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class OnlineTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly GetEmailService _getEmailService;

        public OnlineTransactionController(
           IMediator mediator,
           GetEmailService getEmailService)
        {
            _mediator = mediator;
            _getEmailService = getEmailService;
        }

        private string GetCurrentEmail()
        {
            return _getEmailService.GetEmailFromToken(User);
        }

        private async Task<User> GetCurrentUserByEmail(string email)
        {
            return await _mediator.Send(new GetUserByEmailQuery { Email = email });
        }

        private async Task<Admin> GetCurrentAdminByEmail(string email)
        {
            return await _mediator.Send(new GetAdminByEmailQuery { Email = email });
        }

        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
            try
            {
                var email = GetCurrentEmail();

                var command = new CheckBalanceOnlineCommand { Email = email };
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
        public async Task<IActionResult> Deposit([FromBody] DepositDto depositDto)
        {
            try
            {
                var email = GetCurrentEmail();
                var command = new DepositOnlineCommand { Email = email, Amount = depositDto.Amount };
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
                var email = GetCurrentEmail();
                var command = new TransferOnlineCommand { SenderEmail = email, ReceiverAccountNumber = transferDto.ReceiverAccountNumber, Amount = transferDto.Amount };
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
                var email = GetCurrentEmail();
                var command = new ChangePinOnlineCommand { Email = email, NewPin = changePinDto.NewPin };
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
