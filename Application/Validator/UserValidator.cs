using FluentValidation;
using Domain.Enum;
using System;
using System.Linq;
using Application.Dto;

namespace Application.Validator
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Must(password => password.Length >= 8).WithMessage("Password must be at least 8 characters long")
                .Must(ContainNumberAndSpecialChar).WithMessage("Password must contain at least one number and one special character");

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Must(IsValidName).WithMessage("{PropertyName} contains invalid character");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Must(IsValidName).WithMessage("{PropertyName} contains invalid character");

      
            RuleFor(user => user.Pin)
                .NotEmpty()
                .Must(pin => pin.ToString().Length == 4)
                .WithMessage("Invalid PIN length. PIN must be 4 digits");

        }

        protected bool IsValidName(string name)
        {
            name = name.Replace(" ", "");
            name = name.Replace("-", "");
            return name.All(char.IsLetter);
        }

        private bool ContainNumberAndSpecialChar(string password)
        {
            bool hasNumber = false;
            bool hasSpecialChar = false;

            foreach (var c in password)
            {
                if (char.IsDigit(c))
                {
                    hasNumber = true;
                }
                if (!char.IsLetterOrDigit(c))
                {
                    hasSpecialChar = true;
                }
            }

            return hasNumber && hasSpecialChar;
        }
    }
}
