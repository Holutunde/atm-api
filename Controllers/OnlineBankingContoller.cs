using System.Threading.Tasks;
using ATMAPI.Dto;
using ATMAPI.Interfaces;
using ATMAPI.Models;
using ATMAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class OnlineTransactionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly GetEmailService _getEmailService;

        public OnlineTransactionController(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            GetEmailService getEmailService
        )
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _getEmailService = getEmailService;
        }

        private string GetCurrentEmail()
        {
            return _getEmailService.GetEmailFromToken(User);
        }

        private async Task<User> GetCurrentUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        private async Task<Admin> GetCurrentAdminByEmail(string email)
        {
            return await _adminRepository.GetAdminByEmail(email);
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

            var user =
                await GetCurrentUserByEmail(email) ?? (IUser)await GetCurrentAdminByEmail(email);
            if (user == null)
                return Unauthorized();

            user.Balance += depositDto.Amount;
            if (user is User userObj)
            {
                await _userRepository.UpdateUserBalance(userObj.Id, user.Balance);
            }
            else if (user is Admin adminObj)
            {
                await _adminRepository.UpdateAdminBalance(adminObj.Id, user.Balance);
            }

            return Ok(new { balance = user.Balance });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            var email = GetCurrentEmail();

            var sender =
                await GetCurrentUserByEmail(email) ?? (IUser)await GetCurrentAdminByEmail(email);
            if (sender == null)
                return Unauthorized();

            if (sender.Balance < transferDto.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            var receiver =
                await _userRepository.GetUserByAccountNumber(transferDto.ReceiverAccountNumber)
                ?? (IUser)
                    await _adminRepository.GetAdminByAccountNumber(
                        transferDto.ReceiverAccountNumber
                    );
            if (receiver == null)
            {
                return NotFound("Receiver account not found.");
            }

            sender.Balance -= transferDto.Amount;
            receiver.Balance += transferDto.Amount;

            if (sender is User senderUser)
            {
                await _userRepository.UpdateUserBalance(senderUser.Id, senderUser.Balance);
            }
            else if (sender is Admin senderAdmin)
            {
                await _adminRepository.UpdateAdminBalance(senderAdmin.Id, senderAdmin.Balance);
            }

            if (receiver is User receiverUser)
            {
                await _userRepository.UpdateUserBalance(receiverUser.Id, receiverUser.Balance);
            }
            else if (receiver is Admin receiverAdmin)
            {
                await _adminRepository.UpdateAdminBalance(receiverAdmin.Id, receiverAdmin.Balance);
            }

            return Ok(new { senderBalance = sender.Balance, receiverBalance = receiver.Balance });
        }

        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            var email = GetCurrentEmail();

            var user =
                await GetCurrentUserByEmail(email) ?? (IUser)await GetCurrentAdminByEmail(email);
            if (user == null)
                return Unauthorized();

            user.Pin = changePinDto.NewPin;
            if (user is User userObj)
            {
                await _userRepository.UpdateUserDetails(userObj.Id, new UserDto { Pin = user.Pin });
            }
            else if (user is Admin adminObj)
            {
                await _adminRepository.UpdateAdminDetails(
                    adminObj.Id,
                    new AdminDto { Pin = user.Pin }
                );
            }

            return NoContent();
        }
    }
}
