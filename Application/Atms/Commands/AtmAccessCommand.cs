using Application.Admins.Queries;
using Application.Users.Queries;
using Application.Validator;
using FluentValidation.Results;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;

namespace Application.Atms.Commands
{
    public class AtmAccessCommand : IRequest<(string Token, string ErrorMessage)>
    {
        public long AccountNumber { get; set; }
        public int Pin { get; set; }
    }

    public class AtmAccessCommandHandler : IRequestHandler<AtmAccessCommand, (string Token, string ErrorMessage)>
    {
        private readonly DataContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public AtmAccessCommandHandler(DataContext context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(string Token, string ErrorMessage)> Handle(AtmAccessCommand request, CancellationToken cancellationToken)
        {
            AccessValidator validator = new();
            ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                List<string> errors = result.Errors.Select(error => error.ErrorMessage).ToList();
                string errorMessage = string.Join("\n", errors);
                return (null, errorMessage);
            }

            var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery{AccountNumber= request.AccountNumber}, cancellationToken);

            if (user != null && user.Pin == request.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(user.AccountNumber);
                return (token, null);
            }

            var admin = await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery{AccountNumber = request.AccountNumber}, cancellationToken);

            if (admin != null && admin.Pin == request.Pin)
            {
                var token = _jwtTokenService.GenerateATmToken(admin.AccountNumber);
                return (token, null);
            }

            return (null, "Invalid account number or PIN.");
        }
    }
}
