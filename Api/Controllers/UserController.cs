using Application.Dto;
using Application.Users.Commands;
using Application.Users.Queries;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Application.Admins.Commands;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;

        public UserController(IMediator mediator, IMapper mapper, JwtTokenService jwtTokenService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            try
            {
                var createdUser = await _mediator.Send(command);
                return Created("", new { createdUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserQuery query)
        {
            try
            {
                var user = await _mediator.Send(query);

                if (user == null)
                {
                    return Unauthorized("Invalid credentials.");
                }

                var token = _jwtTokenService.GenerateToken(user.Email, user.Role);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var query = new GetUserByIdQuery { Id = id };
                var user = await _mediator.Send(query);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var query = new GetUserByEmailQuery { Email = email };
                var user = await _mediator.Send(query);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            try
            {
                var updatedUser = await _mediator.Send(command);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
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
