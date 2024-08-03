using Application.Common.ResultsModel;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Online.Commands
{
    public class TransferCommand : IRequest<Result>
    {
        public string SenderEmail { get; set; }
        public long ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
    }
    
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataContext _context;

        public TransferCommandHandler(UserManager<ApplicationUser> userManager, IDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser sender = await _userManager.FindByEmailAsync(request.SenderEmail);
            if (sender == null)
            {
                return Result.Failure<TransferCommand>("Sender not found.");
            }

            ApplicationUser receiver = await _context.Users.FirstOrDefaultAsync(u => u.AccountNumber == request.ReceiverAccountNumber, cancellationToken);
            if (receiver == null)
            {
                return Result.Failure<TransferCommand>("Receiver not found.");
            }
            
            if (sender.AccountNumber == receiver.AccountNumber)
            {
                return Result.Failure<TransferCommand>("Sender cannot transfer to sender account");
            }


            if (sender.Balance < request.Amount)
            {
                return Result.Failure<TransferCommand>("Insufficient funds.");
            }

            // Perform the transfer  
            sender.Balance -= request.Amount;  
            receiver.Balance += request.Amount;  

            // Update the sender  
            IdentityResult updateSenderResult = await _userManager.UpdateAsync(sender);  
            // Create a new receiver instance or attach/track it if needed  
            // Use _context for direct manipulations or entity tracking to avoid conflicts  
    
            // Assuming receiver was not tracked, it's safe to update it here  

            var updateReceiverResult = await _context.SaveChangesAsync(cancellationToken);  
            if (updateSenderResult.Succeeded && updateReceiverResult > 0)  
            {  
                return Result.Success<TransferCommand>("Transfer successful.",  sender.Balance);  
            }  

            return Result.Failure<TransferCommand>("Transfer unsuccessful.");
        }
    }
}
