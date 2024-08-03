using Application.Users.Commands;
//using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Users.Queries;

namespace API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/users")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }
        

        // Get user by ID
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        
        //Get user by email
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var query = new GetUserByEmailQuery { Email = email };
            var result = await _mediator.Send(query);
            return Ok(result);;
        }
        
        // Get user by account number
        [HttpGet("accountnumber/{accountNumber}")]
        public async Task<IActionResult> GetByAccountNumber(long accountNumber)
        {
            var query = new GetUserByAccountNumberQuery() { AccountNumber = accountNumber };
            var result = await _mediator.Send(query);
            return Ok(result);
        
            
        }
        
        [HttpPost("unlock/{email}")]
        public async Task<IActionResult> UnlockUser(string email)
        {
            
            var command = new UnlockUserCommand() { Email = email };
            var result = await _mediator.Send(command);
            return Ok(result);
           
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsersByUserRole()
        {
            var result = await _mediator.Send(new GetAllUsersByUserRoleQuery());
            return Ok(result);
        }

        [HttpGet("all-admins")]
        public async Task<IActionResult> GetAllUsersByAdminRole()
        {
            var result = await _mediator.Send(new GetAllUsersByAdminRoleQuery());
            return Ok(result);
        }

        // Update user balance
        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance(UpdateUserBalanceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        
        // Delete a user
        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            var command = new DeleteUserCommand { Email = email };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
