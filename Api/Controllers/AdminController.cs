using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Validator;
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
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterAdminCommand command)
        {
            AdminValidator validator = new();
            ValidationResult result = validator.Validate(command);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }  

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
        public async Task<IActionResult> Login([FromBody] LoginAdminQuery query)
        {

            var admin = await _mediator.Send(query);

            if (admin == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _jwtTokenService.GenerateToken(admin.Email, admin.Role);

            return Ok(new {admin, token });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateAdminDetails")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminCommand command)
        {   
           var updatedAdmin =  await _mediator.Send(command);

            return Ok(updatedAdmin);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDetails")]
        public async Task<IActionResult> GetAdminById([FromBody] GetAdminByIdQuery query)
        {
        
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
        [HttpDelete("deleteAdmin")]
        public async Task<IActionResult> DeleteAdmin([FromBody] int id )
        {

            var result = await _mediator.Send(new DeleteAdminCommand { Id = id });

            if (result)
            {
                return Ok($"User with ID {id} deleted successfully.");
            }
            else
            {
                return NotFound($"User with ID {id} not found.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] int id)
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