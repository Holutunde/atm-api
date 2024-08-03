using Application.Common.ResultsModel;
using Application.Users.Commands;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;


namespace ATMAPI.Tests.User.Commands
{

    public class UpdateUserBalanceCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly UpdateUserBalanceCommandHandler _handler;

    public UpdateUserBalanceCommandHandlerTests()
    {
        _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
        _handler = new UpdateUserBalanceCommandHandler(_mockUserManager.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        UpdateUserBalanceCommand command = new UpdateUserBalanceCommand { Email = "nonexistent@example.com", NewBalance = 1000.0 };
        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
    {
        // Arrange
        UpdateUserBalanceCommand command = new UpdateUserBalanceCommand { Email = "user@example.com", NewBalance = 1000.0 };
        ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 500.0 };

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Failed to update user's balance: ");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenBalanceIsUpdatedSuccessfully()
    {
        // Arrange
        UpdateUserBalanceCommand command = new UpdateUserBalanceCommand { Email = "user@example.com", NewBalance = 1000.0 };
        ApplicationUser user = new ApplicationUser { Email = command.Email, Balance = 500.0 };

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Message.Should().Be("User balance updated successfully.");
        result.Entity.Should().Be(1000.0);
    }
 }
}