using Application.Dto;
using Application.Interfaces;
using Application.Validator;
using Domain.Entities;
using FluentValidation.Results;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/atm")]
    public class AtmTransactionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;

        private readonly JwtTokenService _jwtTokenService;


     public AtmTransactionController(IUserRepository userRepository, IAdminRepository adminRepository, JwtTokenService jwtTokenService){
            _adminRepository = adminRepository;
            _userRepository = userRepository;
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

            var user = await _userRepository.GetUserByAccountNumber(accessDto.AccountNumber);

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            if (user.Pin == accessDto.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(user.AccountNumber);
                return Ok(new { token });
            }

            var admin = await _adminRepository.GetAdminByAccountNumber(accessDto.AccountNumber);
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

            var user = await _userRepository.GetUserByAccountNumber(accountNumber);
            if (user != null)
            {
                return Ok(new { balance = user.Balance });
            }

            var admin = await _adminRepository.GetAdminByAccountNumber(accountNumber);
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

            var accountNumber = getEmailService.GetAccountNumberFromToken(User); ;

            var user = await _userRepository.GetUserByAccountNumber(accountNumber);
            var admin = user == null ? await _adminRepository.GetAdminByAccountNumber(accountNumber) : null;
            if (user == null && admin == null) return Unauthorized();

            if (user != null)
            {
                user.Balance += depositDto.Amount;
                await _userRepository.UpdateUserBalance(user.Id, user.Balance);
            }
            else if (admin != null)
            {
                admin.Balance += depositDto.Amount;
                await _adminRepository.UpdateAdminBalance(admin.Id, admin.Balance);
            }

            return Ok(new { balance = user?.Balance ?? admin.Balance });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            GetEmailService getEmailService = new();

            var accountNumber = getEmailService.GetAccountNumberFromToken(User); ;

            var sender = await _userRepository.GetUserByAccountNumber(accountNumber);
            var adminSender = sender == null ? await _adminRepository.GetAdminByAccountNumber(accountNumber) : null;
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

            var receiverUser = await _userRepository.GetUserByAccountNumber(transferDto.ReceiverAccountNumber);
            var receiverAdmin = receiverUser == null ? await _adminRepository.GetAdminByAccountNumber(transferDto.ReceiverAccountNumber) : null;
            if (receiverUser == null && receiverAdmin == null)
            {
                return NotFound("Receiver account not found.");
            }

            if (sender != null)
            {
                sender.Balance -= transferDto.Amount;
                await _userRepository.UpdateUserBalance(sender.Id, sender.Balance);
            }
            else if (adminSender != null)
            {
                adminSender.Balance -= transferDto.Amount;
                await _adminRepository.UpdateAdminBalance(adminSender.Id, adminSender.Balance);
            }

            if (receiverUser != null)
            {
                receiverUser.Balance += transferDto.Amount;
                await _userRepository.UpdateUserBalance(receiverUser.Id, receiverUser.Balance);
            }
            else if (receiverAdmin != null)
            {
                receiverAdmin.Balance += transferDto.Amount;
                await _adminRepository.UpdateAdminBalance(receiverAdmin.Id, receiverAdmin.Balance);
            }

            return Ok(new { senderBalance = sender?.Balance ?? adminSender.Balance, receiverBalance = receiverUser?.Balance ?? receiverAdmin.Balance });
        }

        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            GetEmailService getEmailService = new();

            var accountNumber = getEmailService.GetAccountNumberFromToken(User); ;

            var user = await _userRepository.GetUserByAccountNumber(accountNumber);
            var admin = user == null ? await _adminRepository.GetAdminByAccountNumber(accountNumber) : null;
            if (user == null && admin == null) return Unauthorized();

            if (user != null)
            {
                user.Pin = changePinDto.NewPin;
                await _userRepository.UpdateUserDetails(user.Id, new UserDto { Pin = user.Pin });
            }
            else if (admin != null)
            {
                admin.Pin = changePinDto.NewPin;
                await _adminRepository.UpdateAdminDetails(admin.Id, new AdminDto { Pin = admin.Pin });
            }

            return NoContent();
        }
    }
}
