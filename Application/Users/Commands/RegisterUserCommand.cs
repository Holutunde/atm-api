using Domain.Entities;
using Infrastructure.Data;
using MediatR;


namespace Application.Admins.Commands
{
    public class RegisterUserCommand : IRequest<User>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
    {
        private readonly DataContext _context;

        public RegisterUserCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            Random random = new();
            var newUser = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Pin = request.Pin,
                Balance = 0,
                AccountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L,
                OpeningDate = DateTime.Now,
                Role = "User"
            };

            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newUser;
        }
    }
}
