using Application.Users.Commands;
//using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.Common.ResultsModel;
using Application.Users.Queries;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        // Login a user
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        

    }
}
