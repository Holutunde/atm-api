using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Domain.Enum;

namespace Application.Users.Commands
{
    public class LoginUserCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<LoginUserCommand>("Invalid login attempt.");
            }
            
            if (user.UserStatus != Status.Active)
            {
                return Result.Failure<LoginUserCommand>($"User {request.Email} account is not active.");
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);
            
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    user.UserStatus = Status.Suspended;
                    user.UserStatusDes = Status.Suspended.ToString();
                    await _userManager.UpdateAsync(user);

                    return Result.Failure<LoginUserCommand>($"User {request.Email} account locked: Unsuccessful 3 login attempts.");
                }
                return Result.Failure<LoginUserCommand>("Invalid login attempt.");
            }

            string token = _tokenService.GenerateToken(user.Email, user.RoleDesc);
            return Result.Success(token);
        }
    }
}
