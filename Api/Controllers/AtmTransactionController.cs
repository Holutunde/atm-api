using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Dto;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Validator;
using FluentValidation.Results;
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
        private readonly JwtTokenService _jwtTokenService;


     public AtmTransactionController(IMediator mediator, JwtTokenService jwtTokenService)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("access")]
        public async Task<IActionResult> Access([FromBody] AtmLoginDto accessDto)
        {
            AccessValidator validator = new();
            ValidationResult result = validator.Validate(accessDto);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);

                return BadRequest(errorMessage);
            }

            var userQuery = new GetUserByAccountNumberQuery { AccountNumber =  accessDto.AccountNumber };
            var user = await _mediator.Send(userQuery);

            if (user != null && user.Pin == accessDto.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(user.AccountNumber);
                return Ok(new { token });
            }

            var adminQuery = new GetAdminByAccountNumberQuery{ AccountNumber = accessDto.AccountNumber};
            var admin = await _mediator.Send(adminQuery);

            if (admin != null && admin.Pin == accessDto.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(admin.AccountNumber);
                return Ok(new { token });
            }

            return Unauthorized("Invalid account number or PIN.");
        }


        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {

            GetEmailService getEmailService = new();

             var accountNumber = getEmailService.GetAccountNumberFromToken(User); ;

            var userQuery = new GetUserByAccountNumberQuery { AccountNumber = accountNumber };
            var user = await _mediator.Send(userQuery);
            if (user != null)
            {
                return Ok(new { balance = user.Balance });
            }

            var adminQuery = new GetAdminByAccountNumberQuery { AccountNumber = accountNumber };
            var admin = await _mediator.Send(adminQuery);
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
            GetEmailService getEmailService = new();

            var accountNumber = getEmailService.GetAccountNumberFromToken(User);

            var userQuery = new GetUserByAccountNumberQuery { AccountNumber = accountNumber };
            var user = await _mediator.Send(userQuery);

            if (user == null)
            {
                var adminQuery = new GetAdminByAccountNumberQuery { AccountNumber = accountNumber };
                var admin = await _mediator.Send(adminQuery);

                if (admin == null)
                {
                    return Unauthorized();
                }

                admin.Balance += depositDto.Amount;
                await _mediator.Send(new UpdateAdminBalanceCommand { Id = admin.Id, NewBalance = admin.Balance });

                return Ok(new { balance = admin.Balance });
            }

            user.Balance += depositDto.Amount;
            await _mediator.Send(new UpdateUserBalanceCommand { Id = user.Id, NewBalance = user.Balance });

            return Ok(new { balance = user.Balance });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            GetEmailService getEmailService = new();

            var accountNumber = getEmailService.GetAccountNumberFromToken(User); ;

            var sender = await _mediator.Send(new GetUserByAccountNumberQuery { AccountNumber = accountNumber });
            var adminSender = sender == null ? await _mediator.Send(new GetAdminByAccountNumberQuery { AccountNumber = accountNumber }) : null;
            if (sender == null && adminSender == null)
                return Unauthorized();

            var senderBalance = sender?.Balance ?? adminSender.Balance;

            // Check if sender account number is the same as receiver account number
            if (accountNumber == transferDto.ReceiverAccountNumber)
            {
                return BadRequest("Sender and receiver account numbers cannot be the same.");
            }

            if (senderBalance < transferDto.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            var receiverUser = await _mediator.Send(new GetUserByAccountNumberQuery { AccountNumber = transferDto.ReceiverAccountNumber });
            var receiverAdmin = receiverUser == null ? await _mediator.Send(new GetAdminByAccountNumberQuery { AccountNumber = transferDto.ReceiverAccountNumber }) : null;
            if (receiverUser == null && receiverAdmin == null)
            {
                return NotFound("Receiver account not found.");
            }

            if (sender != null)
            {
                await _mediator.Send(new UpdateUserBalanceCommand { Id = sender.Id, NewBalance = sender.Balance - transferDto.Amount });
            }
            else if (adminSender != null)
            {
                await _mediator.Send(new UpdateAdminBalanceCommand { Id = adminSender.Id, NewBalance = adminSender.Balance - transferDto.Amount });
            }

            if (receiverUser != null)
            {
                await _mediator.Send(new UpdateUserBalanceCommand { Id = receiverUser.Id, NewBalance = receiverUser.Balance + transferDto.Amount });
            }
            else if (receiverAdmin != null)
            {
                await _mediator.Send(new UpdateAdminBalanceCommand { Id = receiverAdmin.Id, NewBalance = receiverAdmin.Balance + transferDto.Amount });
            }

            return Ok(new { senderBalance = sender?.Balance ?? adminSender.Balance, receiverBalance = receiverUser?.Balance ?? receiverAdmin.Balance });
        }


        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            GetEmailService getEmailService = new();

            var accountNumber = getEmailService.GetAccountNumberFromToken(User); ;
            var user = await _mediator.Send(new GetUserByAccountNumberQuery { AccountNumber = accountNumber });
            var admin = user == null ? await _mediator.Send(new GetAdminByAccountNumberQuery { AccountNumber = accountNumber }) : null;
            if (user == null && admin == null) return Unauthorized();

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
