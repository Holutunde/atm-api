using Application.Common.ResultsModel;
using Application.Validator;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Application.Interfaces;
using Application.Users.Queries;
using Domain.Enum;
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
    public class RegisterUserCommandHandler(IDataContext context, IValidator<RegisterUserCommand> validator, IAccountService accountService) : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly IAccountService _accountService = accountService;
        private readonly IValidator<RegisterUserCommand> _validator = validator;

        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            ValidationResult result = _validator.Validate(request);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                var errorMessage = string.Join("\n", errors);
                return Result.Failure<RegisterAdminCommand>(errorMessage);
            }
            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);
            
            if (user != null)
                return Result.Failure<RegisterUserCommand>("Email is registered already");


            var newUser = _accountService.CreateAccount<User>(request.Email, request.Password, request.FirstName, request.LastName, request.Pin, Roles.User, Roles.User.ToString());

            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(newUser, "User registered successfully.");
        }
    }
}
