
using Application.Dto;
using Application.Interfaces;
using Application.Online.Commands;
using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class OnlineController(
       IMediator mediator, IGetEmailService getEmailService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private readonly IGetEmailService _getEmailService = getEmailService;
        

        // [Authorize]
        [HttpGet("checkBalance")]
        public async Task<IActionResult> CheckBalance()
        {
         
                var email = _getEmailService.GetEmailFromToken(User);
        
                var command = new CheckBalanceCommand() { Email = email };
                var owner  = await _mediator.Send(command);
        
                return Ok(new { owner });
         
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositWithdrawDto depositDto)
        {
        
                var email = _getEmailService.GetEmailFromToken(User);
                var command = new DepositCommand() { Email = email, Amount = depositDto.Amount };
                var balance = await _mediator.Send(command);

                return Ok(balance);
                
         
        }
        [Authorize]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] DepositWithdrawDto depositDto)
        {
          
                var email = _getEmailService.GetEmailFromToken(User);
                var command = new WithdrawCommand() { Email = email, Amount = depositDto.Amount };
                var balance = await _mediator.Send(command);

                return Ok(balance);
         
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
           
                var email = _getEmailService.GetEmailFromToken(User);
                var command = new TransferCommand { SenderEmail = email, ReceiverAccountNumber = transferDto.ReceiverAccountNumber, Amount = transferDto.Amount };
                var senderBalance= await _mediator.Send(command);
        
                return Ok(senderBalance);
       
        }
         
        [Authorize]
        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin(ChangePinDto changePinDto)
        {
            var email = _getEmailService.GetEmailFromToken(User);

            var command = new ChangeUserPinCommand
            {
                Email = email, CurrentPin = changePinDto.CurrentPin, NewPin = changePinDto.NewPin
            };
            var result = await _mediator.Send(command);
                
                return Ok(result);
        
        }
        
        [Authorize]
        [HttpPut("update-details")]  
        public async Task<IActionResult> UpdateDto([FromBody] UpdateDetailsDto updateDetailsDto)  
        {  
            var email = _getEmailService.GetEmailFromToken(User);   
            var command = new UpdateUserDetailsCommand   
            {   
                Email = email,   
                UpdateDetailsDto = updateDetailsDto   
            };  

            var result = await _mediator.Send(command);  
            
                return Ok(result);  
         
        }
   
    
    }
}
