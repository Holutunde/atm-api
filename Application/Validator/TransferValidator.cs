using Application.Dto;
using FluentValidation;

namespace Application.Validator
{
    public class TransferValidator : AbstractValidator<TransferDto>
    {
        public TransferValidator()
        {
            RuleFor(user => user.ReceiverAccountNumber)
                .NotEmpty()
                .Must(accountNumber => accountNumber.ToString().Length == 10)
                .WithMessage("Invalid Account Number length. Account Number must be 10 digits");

            RuleFor(user => user.Amount)
                .NotEmpty()
                .WithMessage("Amount is required.")
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.")
                .Must(amount => double.TryParse(amount.ToString(), out _))
                .WithMessage("Amount must be a valid number.");
        }
    }
}