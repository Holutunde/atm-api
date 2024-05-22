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
        private readonly JwtTokenService _jwtTokenService;

        public AccountsController(IRegisteredAccountsService registeredAccountsService, IMapper mapper, JwtTokenService jwtTokenService)
        {
            _registeredAccountsService = registeredAccountsService;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
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

                if(newAccount.Pin.ToString().Length != 4 )
                {
                    return BadRequest("Invalid input. Enter valid 4 digit pin ");
                } 
                newAccount.AccountNumber = accountNumber;
                newAccount.Balance = 0;
                newAccount.OpeningDate = DateTime.Now;

                _registeredAccountsService.AddAccount(newAccount);

                return Ok($"Account created successfully with {newAccount.AccountNumber}");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating account: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public IActionResult LoginAccount([FromBody] LoginDto loginAccount)
        {
            try
            {
                var account = _registeredAccountsService.GetAccountByNumber(loginAccount.AccountNumber);

                if (account == null || account.Pin != loginAccount.Pin)
                {
                    return Unauthorized("Invalid account number or PIN");
                }

                // Generate JWT token
                var accesstoken = _jwtTokenService.GenerateToken(account.AccountNumber);

                return Ok(new { Token = accesstoken });

            }
            catch (Exception ex)
            {
               return StatusCode(500, $"Error during login: {ex.Message}");
            }
        }


        [HttpGet("singleAccount/{accountNumber}")]
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

                if (account.Pin.ToString().Length != 4)
                {
                    return BadRequest("Invalid input. Enter valid 4 digit pin ");
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


        // [HttpPatch("patch/{accountNumber}")]
        // public IActionResult PatchAccountDetails([FromRoute] long accountNumber, [FromBody] JsonPatchDocument<AccountUpdateDto> accountPatch)
        // {
        //     try
        //     {
        //         var account = _registeredAccountsService.GetAccountByNumber(accountNumber);

        //         if (account == null)
        //         {
        //             return NotFound($"Account with number {accountNumber} not found.");
        //         }

        //         var accountUpdateDto = _mapper.Map<AccountUpdateDto>(account);
        //         accountPatch.ApplyTo(accountUpdateDto);
        //         _mapper.Map(accountUpdateDto, account);

        //         _registeredAccountsService.UpdateAccount(account);

        //         return Ok("Account patched successfully");
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"Error patching account: {ex.Message}");
        //     }
        // }
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


        
    }

    
}
