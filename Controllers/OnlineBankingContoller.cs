using System.Threading.Tasks;
using ATMAPI.Dto;
using ATMAPI.Interfaces;
using ATMAPI.Models;
using ATMAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class OnlineTransactionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly GetEmailService _getEmailService;

        public OnlineTransactionController(IUserRepository userRepository, IAdminRepository adminRepository, GetEmailService getEmailService)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _getEmailService = getEmailService;
        }

        private async Task<IUser> GetCurrentUser()
        {
            var email = _getEmailService.GetEmailFromToken(User);
            var user = await _userRepository.GetUserByEmail(email);

            if (user != null)
            {
                return user;
            }

            var admin = await _adminRepository.GetAdminByEmail(email);
            return (IUser)admin;
        }

        [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
            var user = await GetCurrentUser();
            if (user == null) return Unauthorized();

            return Ok(new { balance = user.Balance });
        }


        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto depositDto)
        {
            var user = await GetCurrentUser();
            if (user == null) return Unauthorized();

            user.Balance += depositDto.Amount;
            await _userRepository.UpdateUserBalance(user.Id, user.Balance); 

            return Ok(new { balance = user.Balance });
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            var sender = await GetCurrentUser();
            if (sender == null) return Unauthorized();

            if (sender.Balance < transferDto.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            var receiver = await _userRepository.GetUserByAccountNumber(transferDto.ReceiverAccountNumber)
                            ?? (IUser)await _adminRepository.GetAdminByAccountNumber(transferDto.ReceiverAccountNumber);
            if (receiver == null)
            {
                return NotFound("Receiver account not found.");
            }

            sender.Balance -= transferDto.Amount;
            receiver.Balance += transferDto.Amount;

            await _userRepository.UpdateUserBalance(sender.Id, sender.Balance); // Update sender's balance
            if (receiver is User receiverUser)
            {
                await _userRepository.UpdateUserBalance(receiverUser.Id, receiverUser.Balance); // Update receiver's balance
            }
            else if (receiver is Admin receiverAdmin)
            {
                await _adminRepository.UpdateAdminBalance(receiverAdmin.Id, receiverAdmin.Balance); // Update admin's balance
            }

            return Ok(new { senderBalance = sender.Balance, receiverBalance = receiver.Balance });
        }


        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto changePinDto)
        {
            var user = await GetCurrentUser();
            if (user == null) return Unauthorized();

            user.Pin = changePinDto.NewPin;
            await _userRepository.UpdateUserDetails(user.Id, new UserDto { Pin = user.Pin });

            return NoContent();
        }
    }
}
