using Application.Admins.Commands;
using Application.Common.ResultsModel;
using Application.Interfaces;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;


namespace ATMAPI.UnitTests.Admins.Commands
{
    public class UpdateAdminBalanceCommandHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly UpdateAdminBalanceCommandHandler _handler;

        public UpdateAdminBalanceCommandHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new UpdateAdminBalanceCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_GivenInvalidAdminId_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new UpdateAdminBalanceCommand { Id = 1, NewBalance = 1000 };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> ());
            _contextMock.Setup(c => c.Admins).Returns(admins);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("Admin with ID 1 not found.");
        }

        [Fact]
        public async Task Handle_GivenValidAdminId_ShouldUpdateBalanceAndReturnSuccessResult()
        {
            // Arrange
            var adminId = 1;
            var initialBalance = 500;
            var updatedBalance = 1000;
            var admin = new Admin { Id = adminId, Email = "admin@example.com", Password = "password", Balance = initialBalance };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> { admin });
            _contextMock.Setup(c => c.Admins).Returns(admins);
            _contextMock.Setup(c => c.Admins.FindAsync(new object[] { adminId }, CancellationToken.None)).ReturnsAsync(admin);

            var command = new UpdateAdminBalanceCommand { Id = adminId, NewBalance = updatedBalance };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain($"Admin balance updated successfully.");
            admin.Balance.Should().Be(updatedBalance);
            _contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
