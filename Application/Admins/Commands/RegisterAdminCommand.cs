using Domain.Entities;
using Infrastructure.Data;
using MediatR;


namespace Application.Admins.Commands
{
    public class RegisterAdminCommand : IRequest<Admin>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }

    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Admin>
    {
        private readonly DataContext _context;

        public RegisterAdminCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            Random random = new();
            var newAdmin = new Admin
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Pin = request.Pin,
                Balance = 0,
                AccountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L,
                OpeningDate = DateTime.Now,
                Role = "Admin"
            };

            await _context.Admins.AddAsync(newAdmin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newAdmin;
        }
    }
}
