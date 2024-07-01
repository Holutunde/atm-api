using FluentValidation;
using Application.Dto;
using System.Text.RegularExpressions;
using Application.Admins.Commands;

namespace Application.Validator
{
    public class AdminValidator : AbstractValidator<RegisterAdminCommand>
    {
        public AdminValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Must(ContainNumberAndSpecialChar).WithMessage("Password must be at least 8 characters long, contain at least one number and one special character");


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
       
            string pattern = @"^(?=.*\d)(?=.*[@$!%*?&]).{8,}$";

            // Create a Regex object with the pattern
            Regex regex = new(pattern);

            // Check if the password matches the pattern
            return regex.IsMatch(password);
        }


    }
}
