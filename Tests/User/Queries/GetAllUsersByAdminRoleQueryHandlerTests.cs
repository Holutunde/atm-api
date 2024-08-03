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
    public class GetAllUsersByAdminRoleQueryHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly GetAllUsersByAdminRoleQueryHandler _handler;

        public GetAllUsersByAdminRoleQueryHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _handler = new GetAllUsersByAdminRoleQueryHandler(_mockUserManager.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAdminsAreFound()
        {
            // Arrange
            var query = new GetAllUsersByAdminRoleQuery();
            var admins = new List<ApplicationUser>
            {
                new ApplicationUser { Role = Roles.Admin, Email = "admin1@example.com" },
                new ApplicationUser { Role = Roles.Admin, Email = "admin2@example.com" }
            };
            var mockDbSet = ApplicationDbContextText.GetQueryableMockDbSet(admins);
            _mockUserManager.Setup(x => x.Users).Returns(mockDbSet);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var resultData = result.Entity as dynamic;
            resultData.Equals(admins);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNoAdminsAreFound()
        {
            // Arrange
            var query = new GetAllUsersByAdminRoleQuery();
            var admins = new List<ApplicationUser>();
            var mockDbSet = ApplicationDbContextText.GetQueryableMockDbSet(admins);
            _mockUserManager.Setup(x => x.Users).Returns(mockDbSet);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("No users found with the role 'Admin'.");
        }
    }
}
