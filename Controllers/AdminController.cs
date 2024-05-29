using ATMAPI.Models;
using ATMAPI.Services;
using AutoMapper;
using ATMAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {

        private readonly IRegisteredAccountsService _registeredAccountsService;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;

        public AdminController(IRegisteredAccountsService registeredAccountsService, IMapper mapper, JwtTokenService jwtTokenService)
        {
            _registeredAccountsService = registeredAccountsService;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("create")]
        public IActionResult CreateAdmin([FromBody] AccountDto admin)
        {
            Random random = new();
            long accountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L;

            var newAdmin = _mapper.Map<Admin>(admin);

            if (newAdmin.Pin.ToString().Length != 4)
            {
                return BadRequest("Invalid input. Enter valid 4 digit pin");
            }

            var existingAccount = _registeredAccountsService.GetAccountByNumber(accountNumber);
            if (existingAccount != null)
            {
                return BadRequest("Account number already exists.");
            }

            newAdmin.AccountNumber = accountNumber;
            newAdmin.Balance = 0;
            newAdmin.OpeningDate = DateTime.Now;
            newAdmin.Role = "Admin";

            _registeredAccountsService.AddAdmin(newAdmin);

            return Ok(new{ newAdmin });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updateAdminDetails/{accountNumber}")]
        public IActionResult UpdateAdmin(long accountNumber, [FromBody] Admin admin)
        {
            var existingAdmin = _registeredAccountsService.GetAccountByNumber(accountNumber);
            if (existingAdmin == null || existingAdmin.Role != "Admin")
            {
                return NotFound("Admin not found.");
            }

            existingAdmin.FirstName = admin.FirstName;
            existingAdmin.LastName = admin.LastName;
            existingAdmin.Pin = admin.Pin;
            existingAdmin.Balance = admin.Balance;
            existingAdmin.OpeningDate = admin.OpeningDate;

            _registeredAccountsService.UpdateAccount(existingAdmin);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDetails/{accountNumber}")]
        public IActionResult GetAdminByAccountNumber(long accountNumber)
        {
            var admin = _registeredAccountsService.GetAccountByNumber(accountNumber);
            if (admin == null)
            {
                return NotFound("Admin not found.");
            }
            if (admin.Role != "Admin")
            {
                return NotFound($"{accountNumber} is not an Admin.");
            }
            return Ok(admin);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("getUserDetails/{accountNumber}")]
        public IActionResult GetUserByAccountNumber(long accountNumber)
        {
            var user = _registeredAccountsService.GetAccountByNumber(accountNumber);
            if (user == null)
            {
                return NotFound("Admin not found.");
            }
            if (user.Role != "User")
            {
                return NotFound($"{accountNumber} is not a User.");
            }
            return Ok(user);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("allAdmins")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Admin>))]
        public IActionResult GetAllAdmins()
        {
            var accounts = _registeredAccountsService.GetAccounts();
            var admins = accounts.Where(a => a.Role == "Admin").ToList();
            return Ok(new{admins, totalAdmins = admins.Count});
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("allUsers")]
        public IActionResult GetAllUsers()
        {
            var accounts = _registeredAccountsService.GetAccounts();
            var users = accounts.Where(a => a.Role == "User").ToList();
            return Ok(new{users,totalUsers = users.Count});
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("deleteAccount/{accountNumber}")]
        public IActionResult DeleteAdmin(long accountNumber)
        {
            var admin = _registeredAccountsService.GetAccountByNumber(accountNumber);
            if (admin == null || admin.Role != "Admin")
            {
                return NotFound("Admin not found.");
            }

            _registeredAccountsService.DeleteAccount(accountNumber);
            return NoContent();
        }
    }
}
