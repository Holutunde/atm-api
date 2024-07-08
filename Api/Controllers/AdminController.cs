using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Users.Commands;
using Application.Users.Queries;
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

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterAdminCommand command)
        {
            try
            {
                var createdAdmin = await _mediator.Send(command);
                return Ok(new { createdAdmin });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAdminQuery query)
        {
            try
            {
                var (admin, token) = await _mediator.Send(query);

                if (admin == null)
                {
                    return Unauthorized("Invalid credentials.");
                }

                return Ok(new { admin, token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateAdminDetails")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminCommand command)
        {
            try
            {
                var updatedAdmin = await _mediator.Send(command);
                return Ok(updatedAdmin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDetails")]
        public async Task<IActionResult> GetAdminById([FromBody] GetAdminByIdQuery query)
        {
            try
            {
                var admin = await _mediator.Send(query);

                if (admin == null)
                {
                    return NotFound("Admin not found.");
                }

                return Ok(admin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllAdmins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var query = new GetAllAdminsQuery();
                var admins = await _mediator.Send(query);
                return Ok(new { admins, totalAdmins = admins.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var query = new GetAllUsersQuery();
                var users = await _mediator.Send(query);
                return Ok(new { users, totalUsers = users.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteAdmin")]
        public async Task<IActionResult> DeleteAdmin([FromBody] int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
