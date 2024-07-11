using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admins.Commands
{
    public class LoginAdminCommand : IRequest<Result>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginAdminCommandHandler(IDataContext context, IJwtTokenService jwtTokenService, IPasswordHasher passswordHasher) : IRequestHandler<LoginAdminCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IPasswordHasher _passwordHasher = passswordHasher;

        public async Task<Result> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (admin == null || !_passwordHasher.VerifyPassword(request.Password, admin.Password))
            {
                return Result.Failure<LoginAdminCommand>("Invalid credentials.");
            }

            var token = _jwtTokenService.GenerateToken(admin.Email, admin.Role);
            return Result.Success( token , "Login successful.");
        }
    }
}
