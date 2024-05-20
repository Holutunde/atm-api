using AutoMapper;
using ATMAPI.Models;
using ATMAPI.Dto;
using ATMAPI.Services;
using Microsoft.AspNetCore.Mvc;

using System;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountsController : ControllerBase
    {
        private readonly IRegisteredAccountsService _registeredAccountsService;
        private readonly IMapper _mapper;

        public AccountsController(IRegisteredAccountsService registeredAccountsService, IMapper mapper)
        {
            _registeredAccountsService = registeredAccountsService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public IActionResult CreateAccount([FromBody] AccountDto accountCreate)
        {
            try
            {
                // Generate a random 10-digit account number
                Random random = new();
                long accountNumber = (long)(random.NextDouble() * 9000000000) + 1000000000;

                var newAccount = _mapper.Map<Account>(accountCreate);
                newAccount.AccountNumber = accountNumber;
                newAccount.Balance = 0;
                newAccount.OpeningDate = DateTime.Now;

                _registeredAccountsService.AddAccount(newAccount);

                return Ok($"Account created successfully ");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating account: {ex.Message}");
            }
        }

 
        [HttpPatch("patch/{AccountNumber}")]
        public IActionResult PatchAccountDetails(long AccountNumber, [FromBody] AccountUpdateDto accountPatch)
        {
            try
            {
                var account = _registeredAccountsService.GetAccountByNumber(AccountNumber);

               Console.WriteLine(account.FirstName);
                if (account == null)
                {
                    return NotFound($"Account with number {AccountNumber} not found.");
                }
        
                if (accountPatch.FirstName != null)
                    account.FirstName = accountPatch.FirstName;

                if (accountPatch.LastName != null)
                    account.LastName = accountPatch.LastName;

                if (accountPatch.Pin.HasValue)
                    account.Pin = accountPatch.Pin.Value;

                if (accountPatch.Balance.HasValue)
                    account.Balance = accountPatch.Balance.Value;

                _registeredAccountsService.UpdateAccount(account);

                return Ok("Account patched successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error patching account: {ex.Message}");
            }
        }

        [HttpPut("update/{accountNumber}")]
        public IActionResult UpdateAccountDetails(long accountNumber, [FromBody] AccountDto accountUpdate)
        {
            try
            {
                var account = _registeredAccountsService.GetAccountByNumber(accountNumber);

                if (account == null)
                {
                    return NotFound($"Account with number {accountNumber} not found.");
                }

                _mapper.Map(accountUpdate, account);
                _registeredAccountsService.UpdateAccount(account);

                return Ok("Account updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating account: {ex.Message}");
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Account>))]
        public IActionResult GetAccounts()
        {
            try
            {
                List<Account> accounts = _registeredAccountsService.GetAccounts().ToList();

                List<AccountDto> accountDtos = _mapper.Map<List<AccountDto>>(accounts);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(accountDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving accounts: {ex.Message}");
            }
        }

        [HttpGet("singleAccount/{AccountNumber}")]
        [ProducesResponseType(200)]
        public IActionResult GetAccountByAccountNumber(long AccountNumber)
        {
            try
            {
                 var account = _registeredAccountsService.GetAccountByNumber(AccountNumber);

                // var accountDtos = _mapper.Map<List<AccountDto>>(account);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving accounts: {ex.Message}");
            }
        }

        
    }

    
}
