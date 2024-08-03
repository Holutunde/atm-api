using Application.Common.ResultsModel;
using Application.Online.Commands;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace ATMAPI.Tests.Online.Commands
{
    public class CheckBalanceCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly CheckBalanceCommandHandler _handler;

        public CheckBalanceCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _handler = new CheckBalanceCommandHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            CheckBalanceCommand command = new CheckBalanceCommand { Email = "nonexistentuser@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenBalanceIsRetrieved()
        {
            // Arrange
            CheckBalanceCommand command = new CheckBalanceCommand { Email = "user@example.com" };
            ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 100.50 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("Balance retrieved successfully.");
            result.Entity.Should().Be(user.Balance);
        }
    }
    
}
