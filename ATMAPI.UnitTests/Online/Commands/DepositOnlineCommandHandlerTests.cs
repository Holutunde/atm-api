using Application.Online.Commands;
using Application.Interfaces;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace ATMAPI.UnitTests.Online.Commands;

public class DepositOnlineCommandHandlerTests
{
    private readonly Mock<IDataContext> _contextMock;
    private readonly DepositOnlineCommandHandler _handler;

    public DepositOnlineCommandHandlerTests()
    {
        _contextMock = new Mock<IDataContext>();
        _handler = new DepositOnlineCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_GivenValidUserEmail_ShouldDepositToUserBalance()
    {
        // Arrange
        var user = new User { Id = 1, Email = "user@example.com", Balance = 100.0, AccountNumber = 12345 };
        var users =  ApplicationDbContextText.GetQueryableMockDbSet(new List<User> {user});

        _contextMock.Setup(c => c.Users).Returns(users);
       _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new DepositOnlineCommand { Email = "user@example.com", Amount = 50.0 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Entity.Should().Be(150.0);
        result.Message.Should().Be("Deposit successful.");
        user.Balance.Should().Be(150.0);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenValidAdminEmail_ShouldDepositToAdminBalance()
    {
        // Arrange
        var admin = new Admin { Id = 1, Email = "admin@example.com", Balance = 200.0, AccountNumber = 67890 };
        var admins =  ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> {admin});

        _contextMock.Setup(c => c.Admins).Returns(admins);
        _contextMock.Setup(c => c.Users).Returns( ApplicationDbContextText.GetQueryableMockDbSet(new List<User>()));

        var command = new DepositOnlineCommand { Email = "admin@example.com", Amount = 75.0 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Entity.Should().Be(275.0);
        result.Message.Should().Be("Deposit successful.");
        admin.Balance.Should().Be(275.0);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        var users =  ApplicationDbContextText.GetQueryableMockDbSet(new List<User> ());
        var admins =  ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> ());;

        _contextMock.Setup(c => c.Users).Returns(users);
        _contextMock.Setup(c => c.Admins).Returns(admins);

        var command = new DepositOnlineCommand { Email = "invalid@example.com", Amount = 100.0 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Unauthorized");
    }
}


