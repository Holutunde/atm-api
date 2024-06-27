using FluentValidation;
using Application.Dto;
using System.Text.RegularExpressions;

namespace Application.Validator
{
    public class AccessValidator : AbstractValidator<AtmLoginDto>
    {
        public AccessValidator()
        {
            RuleFor(user => user.AccountNumber)
                .NotEmpty()
                .Must(accountNumber => accountNumber.ToString().Length == 10)
                .WithMessage("Invalid Account Number length. Account Number must be 10 digits");
    

            RuleFor(user => user.Pin)
              .NotEmpty()
              .Must(pin => pin.ToString().Length == 4)
              .WithMessage("Invalid PIN length. PIN must be 4 digits");


        }

    }
}
