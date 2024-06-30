using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Dto;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Validator;
using AutoMapper;
using FluentValidation.Results;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly JwtTokenService _jwtTokenService;

        public AdminController(IMediator mediator, JwtTokenService jwtTokenService)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
        }
    

        [HttpPost("register")]
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

            var command = new RegisterAdminCommand
            {
                Email = adminDto.Email,
                Password = adminDto.Password,
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
                Pin = adminDto.Pin
            };

            try
            {
                var createdAdmin = await _mediator.Send(command);
                return Ok(new { createdAdmin });
            }
            catch (Exception )
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OnlineLoginDto loginDto)
        {
            var query = new LoginAdminQuery { LoginDto = loginDto };
            var admin = await _mediator.Send(query);

            if (admin == null)
            {
                return Unauthorized("Invalid credentials.");
            }


            var token = _jwtTokenService.GenerateToken(admin.Email);// Replace with actual JWT token generation
            return Ok(new { token });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updateAdminDetails/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] AdminDto adminDto)
        {
            var command = new UpdateAdminCommand { Id = id, AdminDto = adminDto };
            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDetails/{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var query = new GetAdminByIdQuery { Id = id };
            var admin = await _mediator.Send(query);

            if (admin == null)
            {
                return NotFound("Admin not found.");
            }

            return Ok(admin);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllAdmins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var query = new GetAllAdminsQuery();
            var admins = await _mediator.Send(query);

            return Ok(new { admins, totalAdmins = admins.Count });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);

            return Ok(new { users, totalUsers = users.Count });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteAdmin/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var command = new DeleteAdminCommand { Id = id };
            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command);


            if (result)
            {
                return Ok($"User with ID {id} deleted successfully.");
            }
            else
            {
                return NotFound($"User with ID {id} not found.");
            }
        }
    }
}
