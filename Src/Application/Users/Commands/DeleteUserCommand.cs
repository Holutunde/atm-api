using Application.Common.ResultsModel;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<DeleteUserCommand>("User not found.");
            }

            // Update the user's status to Inactive
            user.UserStatus = Status.Inactive;
            user.UserStatusDes = Status.Inactive.ToString();
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                
                return Result.Failure<DeleteUserCommand>("Failed to update user status.");
               
            }
            return Result.Success("User status updated to Inactive successfully.");
    
        }
    }
}