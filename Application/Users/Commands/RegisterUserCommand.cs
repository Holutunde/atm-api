using Application.Common.ResultsModel;
using Application.Validator;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Application.Interfaces;
using MediatR;


namespace Application.Admins.Commands
{
    public class RegisterUserCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }
    public class RegisterUserCommandHandler(IDataContext context, IValidator<RegisterUserCommand> validator, IAccountFactory accountFactory) : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly IAccountFactory _accountFactory = accountFactory;
        private readonly IValidator<RegisterUserCommand> _validator = validator;

        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            UserValidator validator = new();
            ValidationResult result = _validator.Validate(request);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);
                return Result.Failure<RegisterAdminCommand>(errorMessage);
            }

            var newUser = _accountFactory.CreateAccount<User>(request.Email, request.Password, request.FirstName, request.LastName, request.Pin, "User");

            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(newUser, "User registered successfully.");
        }
    }
}
