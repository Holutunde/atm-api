using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Domain.Entities; 

namespace Application.Users.Commands
{
    public class ChangeUserPinCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public int CurrentPin { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangeUserPinCommandHandler : IRequestHandler<ChangeUserPinCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChangeUserPinCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)  
        {  
            _userManager = userManager;
            _signInManager = signInManager;  
        }  

        public async Task<Result> Handle(ChangeUserPinCommand request, CancellationToken cancellationToken)  
        {  
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);  
            if (user == null)  
            {  
                return Result.Failure<ChangeUserPinCommand>("User not found.");  
            }  

            // Validate the current PIN  
            if (!UserPinHasher.VerifyPin(request.CurrentPin, user.PinHash))  
            {  
                return Result.Failure<ChangeUserPinCommand>("Current PIN is incorrect.");  
            }

            user.PinHash = UserPinHasher.HashPin(request.NewPin);
            IdentityResult updateResult = await _userManager.UpdateAsync(user);  
            if (updateResult.Succeeded)  
            {  
                await _signInManager.RefreshSignInAsync(user);  
                return Result.Success("PIN changed successfully.");  
            }  

            return Result.Failure<ChangeUserPinCommand>("Failed to change PIN.");  
        }  
    }
}