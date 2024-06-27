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
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;

        public UserController(IUserRepository userRepository,
        IMapper mapper,
        JwtTokenService jwtTokenService)
        {
            _userRepository= userRepository;
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


            var newUser = _mapper.Map<User>(userDto);

            var existingUser = await _userRepository.GetUserByEmail(newUser.Email);

            if (existingUser != null)
            {
                return BadRequest("User email already exists.");
            }

            Random random = new Random();
            long accountNumber = GenerateRandomAccountNumber();

            newUser.Balance = 0;
            newUser.OpeningDate = DateTime.Now;
            newUser.AccountNumber = accountNumber;
            newUser.Role = "User";

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            var createdUser = await _userRepository.Register(newUser);

            return Created("", new { createdUser });
        }

        private long GenerateRandomAccountNumber()
        {
            Random random = new Random();
            return (long)(random.NextDouble() * 9000000000L) + 1000000000L;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OnlineLoginDto loginDto)
        {
            var user = await _userRepository.Login(loginDto);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _jwtTokenService.GenerateToken(user.Email);
            return Ok(new { token });
        }

        // [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
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
            var user = await _userRepository.GetUserByEmail(email);
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
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            await _userRepository.UpdateUserDetails(id, userDto);
            return NoContent();
        }
    }
}
