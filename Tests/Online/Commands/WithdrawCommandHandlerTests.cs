using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Online.Commands;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;


namespace ATMAPI.Tests.Online.Commands
{
    public class WithdrawCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IDataContext> _mockDataContext;
        private readonly WithdrawCommandHandler _handler;

        public WithdrawCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _mockDataContext = new Mock<IDataContext>();
            _handler = new WithdrawCommandHandler(_mockUserManager.Object, _mockDataContext.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            WithdrawCommand command = new WithdrawCommand { Email = "nonexistentuser@example.com", Amount = 100.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInsufficientBalance()
        {
            // Arrange
            WithdrawCommand command = new WithdrawCommand { Email = "user@example.com", Amount = 200.0 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 100.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Insufficient balance.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenWithdrawalIsSuccessful()
        {
            // Arrange
            WithdrawCommand command = new WithdrawCommand { Email = "user@example.com", Amount = 50.0 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 100.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("Withdrawal successful.");
            result.Entity.Should().Be(50.0);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            WithdrawCommand command = new WithdrawCommand { Email = "user@example.com", Amount = 50.0 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 100.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Withdrawal unsuccessful.");
        }
    }

}
