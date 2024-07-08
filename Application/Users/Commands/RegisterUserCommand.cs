using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
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

        private readonly IValidator<RegisterUserCommand> _validator;

        public RegisterUserCommandHandler(DataContext context, IValidator<RegisterUserCommand> validator)
        {
            _context = context;
            _validator = validator;
        }
        public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            ValidationResult result = _validator.Validate(request);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);
                throw new ValidationException(errorMessage);
            }

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
