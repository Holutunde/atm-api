using Application.Admins.Commands;
using Application.Common.ResultsModel;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ATMAPI.UnitTests.Mocks;
using Xunit;

namespace ATMAPI.UnitTests.Admins.Commands
{
    public class DeleteAdminCommandHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly DeleteAdminCommandHandler _handler;

        public DeleteAdminCommandHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new DeleteAdminCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_GivenNonExistentAdminId_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new DeleteAdminCommand { Id = 1 };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin>());
            _contextMock.Setup(c => c.Admins).Returns(admins);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain($"Admin with ID {command.Id} not found.");
        }
        
        [Fact]
        public async Task Handle_GivenValidAdminId_ShouldReturnSuccessResult()
        {
            // Arrange
            var adminId = 1;
            var admin = new Admin { Id = adminId, Email = "admin@example.com", Password = "password" };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> { admin });
            _contextMock.Setup(c => c.Admins).Returns(admins);
            _contextMock.Setup(c => c.Admins.FindAsync(adminId)).ReturnsAsync(admin);
            _contextMock.Setup(c => c.Admins.Remove(admin));

            var command = new DeleteAdminCommand { Id = adminId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain($"Admin with ID {adminId} deleted successfully.");
            _contextMock.Verify(c => c.Admins.Remove(admin), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
