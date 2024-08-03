using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Users;
using Application.Users.Commands;
using ATMAPI.Tests.Mock;
using ATMAPI.Tests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace ATMAPI.Tests.User.Commands
{
    public class ChangeUserPinCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly ChangeUserPinCommandHandler _handler;

        public ChangeUserPinCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _mockSignInManager = SignInManagerMock.CreateMockSignInManager<ApplicationUser>(_mockUserManager.Object);
            _handler = new ChangeUserPinCommandHandler(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            ChangeUserPinCommand command = new ChangeUserPinCommand { Email = "nonexistent@example.com", CurrentPin = 1234, NewPin = 5678 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCurrentPinIsIncorrect()
        {
            // Arrange
            ChangeUserPinCommand command = new ChangeUserPinCommand { Email = "user@example.com", CurrentPin = 1234, NewPin = 5678 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, PinHash = UserPinHasher.HashPin(9999) }; // Fake hash for current pin

            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Current PIN is incorrect.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            ChangeUserPinCommand command = new ChangeUserPinCommand { Email = "user@example.com", CurrentPin = 1234, NewPin = 5678 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, PinHash = UserPinHasher.HashPin(command.CurrentPin) };

            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Failed to change PIN.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPinIsChangedSuccessfully()
        {
            // Arrange
            ChangeUserPinCommand command = new ChangeUserPinCommand { Email = "user@example.com", CurrentPin = 1234, NewPin = 5678 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, PinHash = UserPinHasher.HashPin(command.CurrentPin) };

            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            _mockSignInManager.Setup(x => x.RefreshSignInAsync(user)).Returns(Task.CompletedTask);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("PIN changed successfully.");
        }
    }
}
