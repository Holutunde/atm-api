using Application.Admins.Commands;
using Application.Interfaces;
using Moq;
using Domain.Entities;
using Xunit;

namespace Application.UnitTests.Admins.Commands
{
    public class ChangeAdminPinCommandHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly ChangeAdminPinCommandHandler _handler;

        public ChangeAdminPinCommandHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new ChangeAdminPinCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_ValidAdminId_ShouldChangePin()
        {
            // Arrange
            var adminId = 1;
            var newPin = 9999;
            var admin = new Admin() { Id = adminId, Pin = 1234 };
            _contextMock.Setup(c => c.Admins.FindAsync(adminId)).ReturnsAsync(admin);

            var command = new ChangeAdminPinCommand { Id = adminId, NewPin = newPin };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(newPin, admin.Pin);
            _contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidAdminId_ShouldReturnFailure()
        {
            // Arrange
            var adminId = 999; // Non-existent admin ID
            var newPin = 9999;
            _contextMock.Setup(c => c.Admins.FindAsync(adminId)).ReturnsAsync((Admin)null);

            var command = new ChangeAdminPinCommand { Id = adminId, NewPin = newPin };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal($"Admin with ID {adminId} not found.", result.Message);
            _contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Never); // Ensure SaveChangesAsync was not called
        }
    }
}
