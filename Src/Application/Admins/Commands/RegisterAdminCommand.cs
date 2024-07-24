using Application.Admins.Queries;
using Application.Common.ResultsModel;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;


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

    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Result>
    {
        private readonly IDataContext _context;
        private readonly IValidator<RegisterAdminCommand> _validator;
        private readonly IEmailSender _emailSender;
        private readonly IAccountService _accountService;

        public RegisterAdminCommandHandler(IDataContext context, IValidator<RegisterAdminCommand> validator, IEmailSender emailSender, IAccountService accountService)
        {
            _context = context;
            _validator = validator;
            _emailSender = emailSender;
            _accountService =  accountService;
        }

        public async Task<Result> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            ValidationResult result = _validator.Validate(request);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                var errorMessage = string.Join("\n", errors);
                return Result.Failure<RegisterAdminCommand>(errorMessage);
            }

            if (await _context.Admins.AnyAsync(a => a.Email == request.Email, cancellationToken))
                return Result.Failure<RegisterAdminCommand>("Email is registered already");

            var newAdmin = _accountService.CreateAccount<Admin>(request.Email, request.Password, request.FirstName, request.LastName, request.Pin, Roles.Admin, Roles.Admin.ToString());

            await _context.Admins.AddAsync(newAdmin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _emailSender.SendEmailAsync(request.Email, "Registration Successful", "Welcome to our service!");

            return Result.Success<RegisterAdminCommand>( "Admin registered successfully.", newAdmin);
        }
        
        
    }
}
