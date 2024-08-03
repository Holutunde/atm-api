using Application.Common.ResultsModel;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Online.Commands
{
    public class DepositCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public double Amount { get; set; }
    }
    
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataContext _context;

        public DepositCommandHandler(UserManager<ApplicationUser> userManager, IDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<DepositCommand>("User not found.");
            }

            // Update balance
            user.Balance += request.Amount;
            IdentityResult updateResult = await _userManager.UpdateAsync(user);  
            if (!updateResult.Succeeded)  
            { 
                return Result.Failure<DepositCommand>("Deposit unsuccessful.");
          
            }  

            return Result.Success<DepositCommand>("Deposit successful.", user.Balance);
        }
    }
}