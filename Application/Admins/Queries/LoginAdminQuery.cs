using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admins.Queries
{
    public class LoginAdminQuery : IRequest<(Admin, string)>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginAdminQueryHandler : IRequestHandler<LoginAdminQuery, (Admin, string)>
    {
        private readonly DataContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public LoginAdminQueryHandler(DataContext context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(Admin, string)> Handle(LoginAdminQuery request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
            {
                return (null, null);
            }

            var token = _jwtTokenService.GenerateToken(admin.Email, admin.Role);

            return (admin, token);
        }
    }
}
