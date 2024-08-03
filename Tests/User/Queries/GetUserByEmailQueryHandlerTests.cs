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
    public class GetUserByEmailQueryHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly GetUserByEmailQueryHandler _handler;

        public GetUserByEmailQueryHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _handler = new GetUserByEmailQueryHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsFound()
        {
            // Arrange
            var query = new GetUserByEmailQuery { Email = "user@example.com" };
            var user = new ApplicationUser { Email = query.Email };
            _mockUserManager.Setup(x => x.FindByEmailAsync(query.Email)).ReturnsAsync(user);

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
            var query = new GetUserByEmailQuery { Email = "user@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(query.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }
    }
}
