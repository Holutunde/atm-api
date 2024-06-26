using Application.Dto;
using Api.Helpers;
using Application.Interfaces;
using Application.Validator;
using Domain.Entities;
using Infrastructure.Services;
using FluentValidation.Results;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;

        public AdminController(IAdminRepository adminRepository, IMapper mapper, JwtTokenService jwtTokenService)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminDto adminDto)
        {

            AdminValidator validator = new();
            ValidationResult result = validator.Validate(adminDto);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);

                return BadRequest(errorMessage);
            }


            var newAdmin = _mapper.Map<Admin>(adminDto);
            var existingUser = await _adminRepository.GetAdminByEmail(newAdmin.Email);

            if (existingUser != null)
            {
                return BadRequest("User email already exists.");
            }


            Random random = new();
            long AccountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L;

            newAdmin.Balance = 0;
            newAdmin.OpeningDate = DateTime.Now;
            newAdmin.AccountNumber = AccountNumber;
            newAdmin.Role = "Admin";



            // Hash the password after validation
            newAdmin.Password = BCrypt.Net.BCrypt.HashPassword(newAdmin.Password);

            var createdAdmin = await _adminRepository.Register(newAdmin);

            return Ok(new { createdAdmin });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OnlineLoginDto loginDto)
        {
            var admin = await _adminRepository.Login(loginDto);
            if (admin == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _jwtTokenService.GenerateToken(admin.Email);
            return Ok(new { token });
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("updateAdminDetails/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] AdminDto adminDto)
        {
            var existingAdmin = await _adminRepository.GetAdminById(id);
            if (existingAdmin == null || existingAdmin.Role != "Admin")
            {
                return NotFound("Admin not found.");
            }

            await _adminRepository.UpdateAdminDetails(id, adminDto);
            return NoContent();
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDetails/{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var admin = await _adminRepository.GetAdminById(id);
            if (admin == null || admin.Role != "Admin")
            {
                return NotFound("Admin not found.");
            }
            return Ok(admin);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllAdmins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminRepository.GetAllAdmins();
            return Ok(new { admins, totalAdmins = admins.Count });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminRepository.GetAllUsers();
            return Ok(new { users, totalUsers = users.Count });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteAdmin/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminRepository.GetAdminById(id);
            if (admin == null || admin.Role != "Admin")
            {
                return NotFound("Admin not found.");
            }

            await _adminRepository.DeleteAdminAccount(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteUser/{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _adminRepository.GetUserByEmail(email);
            if (user == null || user.Role != "User")
            {
                return NotFound("User not found.");
            }

            await _adminRepository.DeleteUserAccount(email);
            return NoContent();
        }
    }
}
