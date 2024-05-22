using ATMAPI.Dto;
using ATMAPI.Models;
using ATMAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IRegisteredAccountsService _registeredAccountsService;
        private readonly IMapper _mapper;

        public TransactionsController(IRegisteredAccountsService registeredAccountsService, IMapper mapper)
        {
            _registeredAccountsService = registeredAccountsService;
            _mapper = mapper;
        }

        [HttpGet("checkbalance/{accountNumber}")]
        public IActionResult CheckBalance(long accountNumber)
        {
            var account = _registeredAccountsService.GetAccountByNumber(accountNumber);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            return Ok(account.Balance);
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferDto transferDto)
        {
            var sourceAccount = _registeredAccountsService.GetAccountByNumber(transferDto.SourceAccountNumber);
            var targetAccount = _registeredAccountsService.GetAccountByNumber(transferDto.TargetAccountNumber);

            if (sourceAccount == null || targetAccount == null)
            {
                return NotFound("One or both accounts not found");
            }

            if (sourceAccount.Balance < transferDto.Amount)
            {
                return BadRequest("Insufficient funds");
            }

            sourceAccount.Balance -= transferDto.Amount;
            targetAccount.Balance += transferDto.Amount;

          Console.WriteLine(targetAccount.Balance);

            _registeredAccountsService.UpdateAccount(sourceAccount);
            _registeredAccountsService.UpdateAccount(targetAccount);

            return Ok("Transfer successful ");
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositDto depositDto)
        {
            var account = _registeredAccountsService.GetAccountByNumber(depositDto.AccountNumber);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            account.Balance += depositDto.Amount;
            _registeredAccountsService.UpdateAccount(account);

            return Ok("Deposit successful");
        }

        [HttpPatch("changepin")]
        public IActionResult ChangePin([FromBody] ChangePinDto changePinDto)
        {
            var account = _registeredAccountsService.GetAccountByNumber(changePinDto.AccountNumber);
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
    }
}
