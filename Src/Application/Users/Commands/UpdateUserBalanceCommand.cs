using Application.Common.ResultsModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;


namespace Application.Users.Commands
{
    public class UpdateUserBalanceCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public double NewBalance { get; set; }
    }

    public class UpdateUserBalanceCommandHandler : IRequestHandler<UpdateUserBalanceCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserBalanceCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(UpdateUserBalanceCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<UpdateUserBalanceCommand>("User not found.");
            }

            user.Balance = request.NewBalance;
            
            IdentityResult updateResult = await _userManager.UpdateAsync(user);  
            if (!updateResult.Succeeded)  
            {  
                return Result.Failure<UpdateUserBalanceCommand>("Failed to update user's balance: ");  
            }  

            return Result.Success<UpdateUserBalanceCommand>("User balance updated successfully.", user.Balance);
        }
    }
}
