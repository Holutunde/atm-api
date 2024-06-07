using System.Threading.Tasks;
using ATMAPI.Dto;
using ATMAPI.Helpers;
using ATMAPI.Interfaces;
using ATMAPI.Models;
using ATMAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATMAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(IUserRepository userRepository, IMapper mapper, JwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly JwtTokenService _jwtTokenService = jwtTokenService;

     

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            if (userDto.Pin.ToString().Length != 4)
            {
                return BadRequest("Invalid input. Enter valid 4 digit pin");
            }

            if (!ValidationHelper.IsValidEmail(userDto.Email))
            {
                return BadRequest("Invalid email format.");
            }

            if (!ValidationHelper.IsValidPassword(userDto.Password))
            {
                return BadRequest("Password must be at least 7 characters long and contain at least one number and one special character.");
            }

            var newUser = _mapper.Map<User>(userDto);
            var existingUser = await _userRepository.GetUserByEmail(newUser.Email);

            if (existingUser != null)
            {
                return BadRequest("User email already exists.");
            }

            Random random = new();
            long AccountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L;
            newUser.Balance = 0;
            newUser.OpeningDate = DateTime.Now;
            newUser.AccountNumber = AccountNumber;
            newUser.Role = "User";

            var createdUser = await _userRepository.Register(newUser);

            return Ok(new { createdUser });
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

        [Authorize(Roles = "Admin,User")]
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
