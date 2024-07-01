
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Admins.Queries
{
    public class LoginAdminQuery : IRequest<Admin>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginAdminQueryHandler : IRequestHandler<LoginAdminQuery, Admin>
    {
        private readonly DataContext _context;

        public LoginAdminQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Handle(LoginAdminQuery request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
            {
                return null;
            }
            return admin;
        }
    }
}
