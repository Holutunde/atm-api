using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Users.Commands;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Application.Extensions;

namespace ATMAPI.Tests.User.Commands
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IValidator<RegisterUserCommand>> _mockValidator;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _mockEmailSender = new Mock<IEmailSender>();
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _mockValidator = new Mock<IValidator<RegisterUserCommand>>();

            _handler = new RegisterUserCommandHandler(
                _mockEmailSender.Object,
                _mockUserManager.Object
            );
        }

        [Fact]
         public async Task Handle_ShouldReturnFailure_WhenValidationFails()
         {
             // Arrange
             RegisterUserCommand command = new RegisterUserCommand
             {
                 Email = "invalidemail",
                 Password = "short",
                 FirstName = "John123",
                 LastName = "Doe@",
                 Pin = 123,
                 Role = "Manager"
             };
        
             var validationFailures = new List<ValidationFailure>
             {
                 new ValidationFailure("Email", "Invalid email format"),
                 new ValidationFailure("Password", "Password must be at least 8 characters long"),
                 new ValidationFailure("Password", "Password must contain at least one number and one special character"),
                 new ValidationFailure("FirstName", "First name contains invalid character"),
                 new ValidationFailure("LastName", "Last name contains invalid character"),
                 new ValidationFailure("Pin", "Invalid PIN length. PIN must be 4 digits"),
                 new ValidationFailure("Role", "Invalid role specified. Allowed roles are Admin and User")
             };
        
             _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ValidationResult(validationFailures));
        
             // Act
             Result result;
             try
             {
                 await command.ValidateAsync(_mockValidator.Object, CancellationToken.None);
                 result = Result.Success<RegisterUserCommand>("User registered successfully.");
             }
             catch (ValidationException ex)
             {
                 result = Result.Failure<RegisterUserCommand>(string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));
             }
        
             // Assert
             result.Succeeded.Should().BeFalse();
             result.Message.Should().NotBeNullOrEmpty();
             result.Message.Should().Contain("Invalid email format");
             result.Message.Should().Contain("Password must be at least 8 characters long");
             result.Message.Should().Contain("Password must contain at least one number and one special character");
             result.Message.Should().Contain("First name contains invalid character");
             result.Message.Should().Contain("Last name contains invalid character");
             result.Message.Should().Contain("Invalid PIN length. PIN must be 4 digits");
             result.Message.Should().Contain("Invalid role specified. Allowed roles are Admin and User");
         }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailIsAlreadyRegistered()
        {
            // Arrange
            RegisterUserCommand command = new RegisterUserCommand { Email = "test@example.com" };
            _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email))
                .ReturnsAsync(new ApplicationUser());

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("Email is already registered.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            RegisterUserCommand command = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                Pin = 1234,
                Role = "User"
            };

            _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email))
                .ReturnsAsync((ApplicationUser)null);

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act 
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("User registered successfully.");

            // Verify that SendEmailAsync was called 
            _mockEmailSender.Verify(
                es => es.SendEmailAsync(command.Email, "Registration Successful", "Welcome to our service!"),
                Times.Once);
        }
    }
}
