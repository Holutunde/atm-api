
using Application.Common.ResultsModel;
using Application.Users.Commands;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using ATMAPI.Tests.Mock;
using Xunit;

namespace ATMAPI.Tests.User.Commands
{
    public class UnlockUserCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly UnlockUserCommandHandler _handler;

        public UnlockUserCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();

            _handler = new UnlockUserCommandHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenValidationFails()
        {
            // Arrange
            UnlockUserCommand command = new UnlockUserCommand { Email = "invalid@example.com" };
            var validationFailures = new ValidationFailure[] { new ValidationFailure("Email", "Invalid email format") };
            var validationResult = new ValidationResult(validationFailures);
            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Invalid email format");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            UnlockUserCommand command = new UnlockUserCommand { Email = "nonexistent@example.com" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenResetAccessFailedCountFails()
        {
            // Arrange
            UnlockUserCommand command = new UnlockUserCommand { Email = "user@example.com" };
            var user = new ApplicationUser { Email = command.Email };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Failed());

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Failed to reset access failed count.");
        }
        
    }
}

