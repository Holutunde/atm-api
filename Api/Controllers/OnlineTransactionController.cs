using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Dto;
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
            GetEmailService getEmailService
        )
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
            return await _mediator.Send(new GetUserByEmailQuery{Email = email});
        }

        private async Task<Admin> GetCurrentAdminByEmail(string email)
        {
            return await _mediator.Send(new GetAdminByEmailQuery { Email = email });
        }

        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
            var email = GetCurrentEmail();

            var user = await GetCurrentUserByEmail(email);
            if (user != null)
            {
                return Ok(new { balance = user.Balance });
            }

            var admin = await GetCurrentAdminByEmail(email);
            if (admin != null)
            {
                return Ok(new { balance = admin.Balance });
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto depositDto)
        {
            var email = GetCurrentEmail();

            var user = await GetCurrentUserByEmail(email);

            var admin = user == null ? await GetCurrentAdminByEmail(email) : null;
            if (user == null && admin == null)
                return Unauthorized();

            if (user != null)
            {
                user.Balance += depositDto.Amount;
                await _mediator.Send(new UpdateUserBalanceCommand { Id = user.Id, NewBalance = user.Balance });
            }
            else if (admin != null)
            {
                await _mediator.Send(new UpdateAdminBalanceCommand { Id = admin.Id, NewBalance = admin.Balance + depositDto.Amount });
            }

            return Ok(new { balance = user?.Balance ?? admin.Balance });
        }

        [Authorize]
        [HttpPost("transfer")]
public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
{
    var email = GetCurrentEmail();

    var sender = await GetCurrentUserByEmail(email);
    var adminSender = sender == null ? await GetCurrentAdminByEmail(email) : null;
    if (sender == null && adminSender == null)
        return Unauthorized();

    var senderAccountNumber = sender?.AccountNumber ?? adminSender.AccountNumber;
    var senderBalance = sender?.Balance ?? adminSender.Balance;

    // Check if sender account number is the same as receiver account number
    if (senderAccountNumber == transferDto.ReceiverAccountNumber)
    {
        return BadRequest("Sender and receiver account numbers cannot be the same.");
    }

    if (senderBalance < transferDto.Amount)
    {
        return BadRequest("Insufficient balance.");
    }

    var receiverUser = await _mediator.Send(new GetUserByAccountNumberQuery { AccountNumber = transferDto.ReceiverAccountNumber });
    var receiverAdmin = receiverUser == null 
        ? await _mediator.Send(new GetAdminByAccountNumberQuery { AccountNumber = transferDto.ReceiverAccountNumber }) 
        : null;

    if (receiverUser == null && receiverAdmin == null)
    {
        return NotFound("Receiver account not found.");
    }

    // Deduct amount from sender
    if (sender != null)
    {
        sender.Balance -= transferDto.Amount;
        await _mediator.Send(new UpdateUserBalanceCommand { Id = sender.Id, NewBalance = sender.Balance });
    }
    else if (adminSender != null)
    {
        adminSender.Balance -= transferDto.Amount;
        await _mediator.Send(new UpdateAdminBalanceCommand { Id = adminSender.Id, NewBalance = adminSender.Balance });
    }

    // Add amount to receiver
    if (receiverUser != null)
    {
        receiverUser.Balance += transferDto.Amount;
        await _mediator.Send(new UpdateUserBalanceCommand { Id = receiverUser.Id, NewBalance = receiverUser.Balance });
    }
    else if (receiverAdmin != null)
    {
        receiverAdmin.Balance += transferDto.Amount;
        await _mediator.Send(new UpdateAdminBalanceCommand { Id = receiverAdmin.Id, NewBalance = receiverAdmin.Balance });
    }

    // Assuming you have a method to add transactions
    Transaction transaction = new()
    {
        SenderAccountNumber = senderAccountNumber,
        ReceiverAccountNumber = transferDto.ReceiverAccountNumber,
        Amount = transferDto.Amount,
        TransactionDate = DateTime.UtcNow,
        TransactionType = "Transfer"
    };

    await _mediator.Send(new CreateTransactionCommand { Transaction = transaction });

            return Ok(new
    {
        senderBalance = sender?.Balance ?? adminSender.Balance,
        receiverBalance = receiverUser?.Balance ?? receiverAdmin.Balance
    });
}



        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            var email = GetCurrentEmail();

            var user = await GetCurrentUserByEmail(email);
            var admin = user == null ? await GetCurrentAdminByEmail(email) : null;
            if (user == null && admin == null)
                return Unauthorized();

            if (user != null)
            {
                await _mediator.Send(new ChangeUserPinCommand { Id = user.Id, NewPin = changePinDto.NewPin });
            }
            else if (admin != null)
            {
                await _mediator.Send(new ChangeAdminPinCommand { Id = admin.Id, NewPin = changePinDto.NewPin });
            }


            return NoContent();
        }
    }
}