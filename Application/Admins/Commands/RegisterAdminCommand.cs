using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Validator;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;


namespace Application.Admins.Commands
{
    public class RegisterAdminCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }

    public class RegisterAdminCommandHandler(
        IDataContext context,
        IValidator<RegisterAdminCommand> validator,
        IEmailSender emailSender, IAccountFactory accountFactory)
        : IRequestHandler<RegisterAdminCommand, Result>
    {
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IDataContext _context = context;
        private readonly IValidator<RegisterAdminCommand> _validator = validator;
        private readonly IAccountFactory _accountFactory = accountFactory;

        public async Task<Result> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            AdminValidator validator = new();
            ValidationResult result = _validator.Validate(request);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                var errorMessage = string.Join("\n", errors);
                return Result.Failure<RegisterAdminCommand>(errorMessage);
            }
            
            

            var newAdmin = _accountFactory.CreateAccount<Admin>(request.Email, request.Password, request.FirstName, request.LastName, request.Pin, "Admin");

            await _context.Admins.AddAsync(newAdmin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _emailSender.SendEmailAsync(request.Email, "Registration Successful", "Welcome to our service!");

            return Result.Success(newAdmin, "Admin registered successfully.");
        }
    }
}
