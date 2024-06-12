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

            var user = await GetCurrentUserByEmail(email);
            var admin = user == null ? await GetCurrentAdminByEmail(email) : null;
            if (user == null && admin == null)
                return Unauthorized();

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
            var email = GetCurrentEmail();

            var sender = await GetCurrentUserByEmail(email);
            var adminSender = sender == null ? await GetCurrentAdminByEmail(email) : null;
            if (sender == null && adminSender == null)
                return Unauthorized();

            var senderBalance = sender?.Balance ?? adminSender.Balance;
            if (senderBalance < transferDto.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            var receiverUser = await _userRepository.GetUserByAccountNumber(
                transferDto.ReceiverAccountNumber
            );
            var receiverAdmin =
                receiverUser == null
                    ? await _adminRepository.GetAdminByAccountNumber(
                        transferDto.ReceiverAccountNumber
                    )
                    : null;
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

            return Ok(
                new
                {
                    senderBalance = sender?.Balance ?? adminSender.Balance,
                    receiverBalance = receiverUser?.Balance ?? receiverAdmin.Balance
                }
            );
        }

        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            var email = GetCurrentEmail();

            var user = await GetCurrentUserByEmail(email);
            var admin = user == null ? await GetCurrentAdminByEmail(email) : null;
            if (user == null && admin == null)
                return Unauthorized();

            if (user != null)
            {
                user.Pin = changePinDto.NewPin;
                await _userRepository.UpdateUserDetails(user.Id, new UserDto { Pin = user.Pin });
            }
            else if (admin != null)
            {
                admin.Pin = changePinDto.NewPin;
                await _adminRepository.UpdateAdminDetails(
                    admin.Id,
                    new AdminDto { Pin = admin.Pin }
                );
            }

            return NoContent();
        }
    }
}
