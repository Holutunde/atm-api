using Application.Common.ResultsModel;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Online.Commands
{
    public class CheckBalanceCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }
    
    public class CheckBalanceCommandHandler : IRequestHandler<CheckBalanceCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckBalanceCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(CheckBalanceCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<CheckBalanceCommand>("User not found.");
            }

            return Result.Success<CheckBalanceCommand>("Balance retrieved successfully.", user.Balance);
        }
    }
}