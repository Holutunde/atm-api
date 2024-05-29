using AutoMapper;
using ATMAPI.Models;
using ATMAPI.Dto;
using ATMAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IRegisteredAccountsService _registeredAccountsService;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;

        public AuthenticationController(IRegisteredAccountsService registeredAccountsService, IMapper mapper, JwtTokenService jwtTokenService)
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
                Random random = new();
                long accountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L;

                var newAccount = _mapper.Map<Account>(accountCreate);

                if (newAccount.Pin.ToString().Length != 4)
                {
                    return BadRequest("Invalid input. Enter valid 4 digit pin");
                }
                newAccount.AccountNumber = accountNumber;
                newAccount.Balance = 0;
                newAccount.OpeningDate = DateTime.Now;

                _registeredAccountsService.AddAccount(newAccount);

                return Ok(newAccount);
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

                var accountDetails = _mapper.Map<AccountDto>(account);
              
                var accessToken = _jwtTokenService.GenerateToken(account.AccountNumber, account.Role);

                return Ok(new {accountDetails, accessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during login: {ex.Message}");
            }
        }
    }
}
