using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Common.ResultsModel;


namespace Application.Users.Commands
{
    public class LoginUserCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserCommandHandler(IDataContext context, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher) : IRequestHandler<LoginUserCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return Result.Failure<LoginUserCommand>("Invalid credentials.");
            }

            var token = _jwtTokenService.GenerateToken(user.Email, user.RoleDesc);
            return Result.Success(new { user, token }, "Login successful.");
        }
    }
}
