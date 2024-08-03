using Application.Common.ResultsModel;
using Application.Users.Commands;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ATMAPI.Tests.User.Commands
{
    public class DeleteUserCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
        _handler = new DeleteUserCommandHandler(_mockUserManager.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        DeleteUserCommand command = new DeleteUserCommand { Email = "nonexistent@example.com" };
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
        DeleteUserCommand command = new DeleteUserCommand { Email = "user@example.com" };
        ApplicationUser user = new ApplicationUser { Email = command.Email };
        _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Failed to update user status.");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateSucceeds()
    {
        // Arrange
        DeleteUserCommand command = new DeleteUserCommand { Email = "user@example.com" };
        ApplicationUser user = new ApplicationUser { Email = command.Email };
        _mockUserManager.Setup(um => um.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Message.Should().Be("User status updated to Inactive successfully.");
    }
}
}