using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.ResultsModel;
using Application.Interfaces;
using Application.Users.Queries;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ATMAPI.Tests.User.Queries
{
    public class GetUserByAccountNumberQueryHandlerTests
    {
        private readonly Mock<IDataContext> _mockContext;
        private readonly GetUserByAccountNumberQueryHandler _handler;

        public GetUserByAccountNumberQueryHandlerTests()
        {
            _mockContext = new Mock<IDataContext>();
            _handler = new GetUserByAccountNumberQueryHandler(_mockContext.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsFound()
        {
            // Arrange
            var query = new GetUserByAccountNumberQuery { AccountNumber = 1234567890L };
            var user = new ApplicationUser { AccountNumber = query.AccountNumber };
            var users = ApplicationDbContextText.GetQueryableMockDbSet(new List<ApplicationUser> { user });

            _mockContext.Setup(c => c.Users).Returns(users);

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
            var query = new GetUserByAccountNumberQuery { AccountNumber = 1234567890L };
            var users = ApplicationDbContextText.GetQueryableMockDbSet(new List<ApplicationUser> ());

            _mockContext.Setup(c => c.Users).Returns(users);

            // Act
            Result result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found.");
        }
    }
}
