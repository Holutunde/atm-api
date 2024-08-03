using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.ResultsModel;
using Application.Users.Queries;
using ATMAPI.Tests.Mock;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using Domain.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ATMAPI.Tests.User.Queries
{
    public class GetAllUsersByUserRoleQueryHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly GetAllUsersByUserRoleQueryHandler _handler;

        public GetAllUsersByUserRoleQueryHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _handler = new GetAllUsersByUserRoleQueryHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUsersAreFound()
        {
            // Arrange
            var query = new GetAllUsersByUserRoleQuery();
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Role = Roles.User, Email = "user1@example.com" },
                new ApplicationUser { Role = Roles.User, Email = "user2@example.com" }
            };
            var mockDbSet = ApplicationDbContextText.GetQueryableMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockDbSet);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var resultData = result.Entity as dynamic;
            resultData.Equals(users);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNoUsersAreFound()
        {
            // Arrange
            var query = new GetAllUsersByUserRoleQuery();
            var users = new List<ApplicationUser>();
            var mockDbSet = ApplicationDbContextText.GetQueryableMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockDbSet);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("No users found with the role 'User'.");
        }
    }
}
