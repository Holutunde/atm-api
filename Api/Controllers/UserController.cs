using Application.Dto;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Validator;
using Infrastructure.Services;
using FluentValidation.Results;
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
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            UserValidator validator = new UserValidator();
            ValidationResult result = validator.Validate(userDto);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join(", ", errors);
                return BadRequest(errorMessage);
            }

            var command = new RegisterUserCommand
            {
                Email = userDto.Email,
                Password = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Pin = userDto.Pin
            };

            try
            {
                var createdUser = await _mediator.Send(command);
                return Created("", new { createdUser });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OnlineLoginDto loginDto)
        {
            var query = new LoginUserQuery { LoginDto = loginDto };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _jwtTokenService.GenerateToken(user.Email);
            return Ok(new { token });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var query = new GetUserByEmailQuery { Email = email };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            var command = new UpdateUserCommand { Id = id, UserDto = userDto };
            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
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
