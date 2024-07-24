using Application.Admins.Queries;
using Application.Interfaces;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;

using Moq;
using Xunit;

namespace Application.UnitTests.Admins.Queries
{
    public class GetAdminByEmailQueryHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly GetAdminByEmailQueryHandler _handler;

        public GetAdminByEmailQueryHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new GetAdminByEmailQueryHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_GivenValidEmail_ShouldReturnAdmin()
        {
            // Arrange
            var admin = new Admin { Id = 1, Email = "admin@example.com", Password = "password" };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> { admin });

            _contextMock.Setup(c => c.Admins).Returns(admins);

            var query = new GetAdminByEmailQuery { Email = "admin@example.com" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(admin);
        }

        [Fact]
        public async Task Handle_GivenInvalidEmail_ShouldReturnNull()
        {
            // Arrange
            var admin = new Admin { Id = 1, Email = "admin@example.com", Password = "password" };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> { admin });

            _contextMock.Setup(c => c.Admins).Returns(admins);

            var query = new GetAdminByEmailQuery { Email = "invalid@example.com" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }

}

