
using ATMAPI.Dto;
using ATMAPI.Interfaces;
using ATMAPI.Models;
using ATMAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/atm")]
    public class AtmTransactionController(IUserRepository userRepository, IAdminRepository adminRepository, JwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAdminRepository _adminRepository = adminRepository;

        private readonly JwtTokenService _jwtTokenService = jwtTokenService;




        [HttpPost("access")]
        public async Task<IActionResult> Access([FromBody] AtmLoginDto accessDto)
        {
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

            var senderBalance = sender?.Balance ?? adminSender.Balance;
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
