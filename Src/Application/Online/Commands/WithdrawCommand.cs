using Application.Common.ResultsModel;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Online.Commands
{
    public class WithdrawCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public double Amount { get; set; }
    }

    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataContext _context;

        public WithdrawCommandHandler(UserManager<ApplicationUser> userManager, IDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Result> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<WithdrawCommand>("User not found.");
            }

            // Check if the user has sufficient balance
            if (user.Balance < request.Amount)
            {
                return Result.Failure<WithdrawCommand>("Insufficient balance.");
            }

            // Update balance
            user.Balance -= request.Amount;
            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                
                return Result.Failure<WithdrawCommand>("Withdrawal unsuccessful.");
            }

            return Result.Success<WithdrawCommand>("Withdrawal successful.", user.Balance);
        }
    }
}
