using Application.Admins.Queries;
using Application.Common.ResultsModel;
using Application.Users.Queries;
using Application.Validator;
using FluentValidation.Results;
using Application.Interfaces;
using MediatR;

namespace Application.Atms.Commands
{
    public class AtmAccessCommand : IRequest<Result>
    {
        public long AccountNumber { get; set; }
        public int Pin { get; set; }
    }

    public class AtmAccessCommandHandler(IDataContext context, IJwtTokenService jwtTokenService) : IRequestHandler<AtmAccessCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<Result> Handle(AtmAccessCommand request, CancellationToken cancellationToken)
        {
            AccessValidator validator = new();
            ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);
                return Result.Failure<AtmAccessCommand>(errorMessage);
            }

            var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);

            if (user != null && user.Pin == request.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(user.AccountNumber);
                return Result.Success(token, "ATM token generated successfully.");
            }

            var admin = await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery{AccountNumber = request.AccountNumber}, cancellationToken);

            if (admin != null && admin.Pin == request.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(admin.AccountNumber);
                return Result.Success(token, "ATM token generated successfully.");
            }

            return Result.Failure<AtmAccessCommand>("Invalid account number or PIN.");
        }
    }
}
