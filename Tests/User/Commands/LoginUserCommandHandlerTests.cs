using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Entities;
using Domain.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using ATMAPI.Tests.Mock;
using Xunit;
using ATMAPI.Tests.Mocks;

namespace ATMAPI.Tests.User.Commands
{
    public class LoginUserCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _mockSignInManager = SignInManagerMock.CreateMockSignInManager<ApplicationUser>(_mockUserManager.Object);
            _mockTokenService = new Mock<ITokenService>();

            _handler = new LoginUserCommandHandler(
                _mockSignInManager.Object,
                _mockUserManager.Object,
                _mockTokenService.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            LoginUserCommand command = new LoginUserCommand { Email = "nonexistent@example.com", Password = "Password123!" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Invalid login attempt.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserAccountIsNotActive()
        {
            // Arrange
            LoginUserCommand command = new LoginUserCommand { Email = "inactive@example.com", Password = "Password123!" };
            ApplicationUser inactiveUser = new ApplicationUser { Email = command.Email, UserStatus = Status.Inactive };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(inactiveUser);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be($"User {command.Email} account is not active.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPasswordSignInFails()
        {
            // Arrange
            LoginUserCommand command = new LoginUserCommand { Email = "test@example.com", Password = "WrongPassword123!" };
            ApplicationUser user = new ApplicationUser { Email = command.Email, UserStatus = Status.Active };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(user, command.Password, false, true))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Invalid login attempt.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureAndLockAccount_WhenAccountIsLockedOut()
        {
            // Arrange
            LoginUserCommand command = new LoginUserCommand { Email = "locked@example.com", Password = "Password123!" };
            ApplicationUser user = new ApplicationUser { Email = command.Email, UserStatus = Status.Active };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(user, command.Password, false, true))
                .ReturnsAsync(SignInResult.LockedOut);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be($"User {command.Email} account locked: Unsuccessful 3 login attempts.");
            _mockUserManager.Verify(um => um.UpdateAsync(It.Is<ApplicationUser>(u => u.UserStatus == Status.Suspended)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenLoginIsSuccessful()
        {
            // Arrange
            LoginUserCommand command = new LoginUserCommand { Email = "test@example.com", Password = "Password123!" };
            ApplicationUser user = new ApplicationUser { Email = command.Email, UserStatus = Status.Active, RoleDesc = "User" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(user, command.Password, false, true))
                .ReturnsAsync(SignInResult.Success);
            _mockTokenService.Setup(ts => ts.GenerateToken(user.Email, user.RoleDesc)).Returns("token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("token");
        }
    }
}
