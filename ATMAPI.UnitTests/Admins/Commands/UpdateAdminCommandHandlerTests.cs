using Application.Admins.Commands;
using Domain.Entities;
using Application.Interfaces;
using FluentAssertions;
using Moq;


namespace ATMAPI.UnitTests.Admins.Commands
{
    public class UpdateAdminCommandHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly UpdateAdminCommandHandler _handler;

        public UpdateAdminCommandHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new UpdateAdminCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_GivenValidCommand_ShouldReturnSuccessResult()
        {
            // Arrange
            var admin = new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password", Pin = 1234 };
            _contextMock.Setup(c => c.Admins.FindAsync(1)).ReturnsAsync(admin);

            var command = new UpdateAdminCommand
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "newpassword",
                Pin = 5678
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("Admin details updated successfully.");
            result.Entity.Should().BeEquivalentTo(admin, options => options.ExcludingMissingMembers());
            _contextMock.Verify(c => c.Admins.Update(admin), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenInvalidAdminId_ShouldReturnFailureResult()
        {
            // Arrange
            _contextMock.Setup(c => c.Admins.FindAsync(1)).ReturnsAsync((Admin)null);

            var command = new UpdateAdminCommand
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "newpassword",
                Pin = 5678
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Admin with ID 1 not found.");
            _contextMock.Verify(c => c.Admins.Update(It.IsAny<Admin>()), Times.Never);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
