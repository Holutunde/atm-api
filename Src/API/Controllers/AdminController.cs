using Application.Common.ResultsModel;
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
    public class AdminController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("register")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterAdminCommand command)
        {
            try
            {
                var createdAdmin = await _mediator.Send(command);
                return Ok(createdAdmin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<RegisterAdminCommand>(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAdminCommand command)
        {
            try
            {
                var token = await _mediator.Send(command);

                return Ok (token );
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<LoginAdminCommand>(ex.Message));
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
                return StatusCode(500, Result.Failure<UpdateAdminCommand>( ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAdminDetails")]
        public async Task<IActionResult> GetAdminById([FromQuery] GetAdminByIdQuery query)
        {
            try
            {
                var admin = await _mediator.Send(query);

                return Ok(admin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<GetAdminByIdQuery>( ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllAdmins")]
        public async Task<IActionResult> GetAllAdmins([FromQuery] GetAllAdminsQuery query)
        {
            try
            {
                var admins = await _mediator.Send(query);
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<GetAllAdminsQuery>( ex.Message));
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
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<GetAllUsersQuery>(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteAdmin")]
        public async Task<IActionResult> DeleteAdmin([FromBody] int id)
        {
            try
            {
             await _mediator.Send(new DeleteAdminCommand { Id = id });

            return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<DeleteAdminCommand>( ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] int id)
        {
            try
            {
               var command = new DeleteUserCommand { Id = id };
               await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure<DeleteUserCommand>( ex.Message));
            }
        }
    }
}
