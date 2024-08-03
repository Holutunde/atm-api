using System.Threading;
using System.Threading.Tasks;
using Application.Common.ResultsModel;
using Application.Users.Queries;
using ATMAPI.Tests.Mock;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace ATMAPI.Tests.User.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _handler = new GetUserByIdQueryHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsFound()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = "user-id-123" };
            var user = new ApplicationUser { Id = query.Id };
            
            _mockUserManager.Setup(x => x.FindByIdAsync(query.Id)).ReturnsAsync(user);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Entity.Should().Be(user);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = "user-id-123" };
            _mockUserManager.Setup(x => x.FindByIdAsync(query.Id)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }
    }
}
