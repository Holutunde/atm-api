using AutoMapper;
using ATMAPI.Models;
using ATMAPI.Dto;
using ATMAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {
        private readonly IRegisteredAccountsService _registeredAccountsService;
        private readonly IMapper _mapper;

        public UserController(IRegisteredAccountsService registeredAccountsService, IMapper mapper)
        {
            _registeredAccountsService = registeredAccountsService;
            _mapper = mapper;
        }

        [HttpGet("accountDetails/{accountNumber}")]
        [ProducesResponseType(200)]
        public IActionResult GetAccountByAccountNumber(long accountNumber)
        {
            try
            {
                var account = _registeredAccountsService.GetAccountByNumber(accountNumber);
                if (account == null)
                {
                    return NotFound($"Account with number {accountNumber} not found.");
                }

                var accountDto = _mapper.Map<AccountDto>(account);

                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving account: {ex.Message}");
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

                if (accountUpdate.Pin.ToString().Length != 4)
                {
                    return BadRequest("Invalid input. Enter valid 4 digit pin");
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

 
    }
}
