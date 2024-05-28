using ATMAPI.Dto;
using ATMAPI.Models;
using ATMAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IRegisteredAccountsService _registeredAccountsService;
        private readonly IMapper _mapper;

        public TransactionsController(IRegisteredAccountsService registeredAccountsService, IMapper mapper)
        {
            _registeredAccountsService = registeredAccountsService;
            _mapper = mapper;
        }


        [HttpGet("checkbalance")]
        public IActionResult CheckBalance()
        {
            try
            {
                GetAccountNumberService getAccountNumberService = new();
                long accountNumber = getAccountNumberService.GetAccountNumberFromToken(User);
                var account = _registeredAccountsService.GetAccountByNumber(accountNumber);

                if (account == null)
                {
                    return NotFound("Account not found");
                }

                return Ok($"Your account balance is {account.Balance}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferDto transferDto)
        {
            try
            {
                GetAccountNumberService getAccountNumberService = new();
                long sourceAccountNumber = getAccountNumberService.GetAccountNumberFromToken(User);
                var sourceAccount = _registeredAccountsService.GetAccountByNumber(sourceAccountNumber);
                var targetAccount = _registeredAccountsService.GetAccountByNumber(transferDto.TargetAccountNumber);

                if (sourceAccount == null || targetAccount == null)
                {
                    return NotFound("One or both accounts not found");
                }

                if(sourceAccountNumber == targetAccount.AccountNumber)
                {
                    return BadRequest("Cannot transfer to the same source account");
                }

                if (sourceAccount.Balance < transferDto.Amount)
                {
                    return BadRequest("Insufficient funds");
                }

                sourceAccount.Balance -= transferDto.Amount;
                targetAccount.Balance += transferDto.Amount;

                _registeredAccountsService.UpdateAccount(sourceAccount);
                _registeredAccountsService.UpdateAccount(targetAccount);

                return Ok($"{transferDto.Amount} transferred successful to {targetAccount.AccountNumber}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositDto depositDto)
        {
            try
            {
                GetAccountNumberService getAccountNumberService = new();
                long accountNumber = getAccountNumberService.GetAccountNumberFromToken(User);
                var account = _registeredAccountsService.GetAccountByNumber(accountNumber);
                if (account == null)
                {
                    return NotFound("Account not found");
                }

                account.Balance += depositDto.Amount;
                _registeredAccountsService.UpdateAccount(account);

                return Ok($"Deposit successful. Account balance is currently {account.Balance}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("changepin")]
        public IActionResult ChangePin([FromBody] ChangePinDto changePinDto)
        {
            try
            {
                GetAccountNumberService getAccountNumberService = new();
                long accountNumber = getAccountNumberService.GetAccountNumberFromToken(User);
                var account = _registeredAccountsService.GetAccountByNumber(accountNumber);
                if (account == null)
                {
                    return NotFound("Account not found");
                }

                if (account.Pin != changePinDto.OldPin)
                {
                    return BadRequest("Old PIN is incorrect");
                }

                account.Pin = changePinDto.NewPin;
                _registeredAccountsService.UpdateAccount(account);

                return Ok("PIN changed successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
