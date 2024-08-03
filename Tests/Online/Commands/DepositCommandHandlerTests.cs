using System.Threading;
using System.Threading.Tasks;
using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Online.Commands;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace ATMAPI.Tests.Online.Commands
{
    public class DepositCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IDataContext> _mockDataContext;
        private readonly DepositCommandHandler _handler;

        public DepositCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _mockDataContext = new Mock<IDataContext>();
            _handler = new DepositCommandHandler(_mockUserManager.Object, _mockDataContext.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            DepositCommand command = new DepositCommand { Email = "nonexistentuser@example.com", Amount = 100.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenDepositIsSuccessful()
        {
            // Arrange
            DepositCommand command = new DepositCommand { Email = "user@example.com", Amount = 100.0 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 200.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("Deposit successful.");
            result.Entity.Should().Be(300.0); 
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            DepositCommand command = new DepositCommand { Email = "user@example.com", Amount = 100.0 };
            ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 200.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Deposit unsuccessful.");
        }
    }
    
}
