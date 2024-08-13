using Application.Common.ResultsModel;
using Application.Extensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;

namespace Application.Users.Commands
{
    public class UnlockUserCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UnlockUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
        await request.ValidateAsync(new UnlockUserCommandValidator(), cancellationToken);
          

            // Find the user by email
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<UnlockUserCommand>("User not found.");
            }

            // Reset the access failed count and unlock the user
            IdentityResult resetAccessFailedCountResult = await _userManager.ResetAccessFailedCountAsync(user);
            if (!resetAccessFailedCountResult.Succeeded)
            {
                return Result.Failure<UnlockUserCommand>("Failed to reset access failed count.");
            }

            IdentityResult unlockUserResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            if (!unlockUserResult.Succeeded)
            {
                return Result.Failure<UnlockUserCommand>("Failed to unlock the user.");
            }

            return Result.Success("User account has been unlocked successfully.");
        }

    }
}
