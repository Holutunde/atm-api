using ATMAPI.UnitTests.Mocks;

namespace ATMAPI.UnitTests.Online.Commands;

using Application.Online.Commands;
using Application.Interfaces;
using Application.Users.Queries;
using Application.Admins.Queries;
using Application.Users.Commands;
using Application.Admins.Commands;
using Application.Common.ResultsModel;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ChangePinOnlineCommandHandlerTests
{
    private readonly Mock<IDataContext> _contextMock;
    private readonly ChangePinOnlineCommandHandler _handler;

    public ChangePinOnlineCommandHandlerTests()
    {
        _contextMock = new Mock<IDataContext>();
        _handler = new ChangePinOnlineCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_GivenValidUserEmail_ShouldChangeUserPin()
    {
        // Arrange
        var user = new User { Id = 1, Email = "user@example.com" };
        var users =  ApplicationDbContextText.GetQueryableMockDbSet(new List<User> {user});

        _contextMock.Setup(c => c.Users).Returns(users);
        _contextMock.Setup(c => c.Admins).Returns( ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> ()));

        var command = new ChangePinOnlineCommand { Email = "user@example.com", NewPin = 1234 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Message.Should().Be("PIN changed successfully");
    }

    [Fact]
    public async Task Handle_GivenValidAdminEmail_ShouldChangeAdminPin()
    {
        // Arrange
        var admin = new Admin { Id = 1, Email = "admin@example.com" };
        var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> {admin});

        _contextMock.Setup(c => c.Admins).Returns(admins);
        _contextMock.Setup(c => c.Users).Returns( ApplicationDbContextText.GetQueryableMockDbSet(new List<User>()));

        var command = new ChangePinOnlineCommand { Email = "admin@example.com", NewPin = 1234 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Message.Should().Be("PIN changed successfully");
    }

    [Fact]
    public async Task Handle_GivenInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        _contextMock.Setup(c => c.Users).Returns(ApplicationDbContextText.GetQueryableMockDbSet(new List<User>()));
        _contextMock.Setup(c => c.Admins).Returns(ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin>()));

        var command = new ChangePinOnlineCommand { Email = "invalid@example.com", NewPin = 1234 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }
}
