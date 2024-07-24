
using Application.Admins.Queries;
using Domain.Entities;
using Application.Interfaces;
using Application.Online.Commands;
using Application.Users.Queries;
using ATMAPI.UnitTests.Mocks;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace ATMAPI.UnitTests.Online.Commands
{
 public class CheckBalanceCommandHandlerTests
{
    private readonly Mock<IDataContext> _contextMock;
    private readonly CheckBalanceCommandHandler _handler;

    public CheckBalanceCommandHandlerTests()
    {
        _contextMock = new Mock<IDataContext>();
        _handler = new CheckBalanceCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_GivenValidAdminEmail_ShouldReturnAdminBalance()
    {
        // Arrange
        var admin = new Admin { Email = "admin@example.com", Balance = 200.0 };
        var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> {admin});

        _contextMock.Setup(c => c.Admins).Returns(admins);

        var command = new CheckBalanceOnlineCommand { Email = "admin@example.com" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Entity.Should().Be(200.0);
        result.Message.Should().Be("Balance retrieved successfully.");
    }
    
    [Fact]
    public async Task Handle_GivenValidUserEmail_ShouldReturnUserBalance()
    {
        // Arrange
        var user = new User { Email = "user@example.com", Balance = 100.0 };
        var users = ApplicationDbContextText.GetQueryableMockDbSet(new List<User> { user });

        _contextMock.Setup(c => c.Users).Returns(users);

        var command = new CheckBalanceOnlineCommand { Email = "user@example.com" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Entity.Should().Be(100.0);
        result.Message.Should().Be("Balance retrieved successfully.");
    }



    [Fact]
    public async Task Handle_GivenInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        var users =  ApplicationDbContextText.GetQueryableMockDbSet(new List<User> ());
        var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> ());

        _contextMock.Setup(c => c.Users).Returns(users);
        _contextMock.Setup(c => c.Admins).Returns(admins);

        var command = new CheckBalanceOnlineCommand { Email = "invalid@example.com" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }
  }
}
