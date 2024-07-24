using System.Threading;
using System.Threading.Tasks;
using Application.Admins.Queries;
using Application.Interfaces;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Admins.Queries
{
    public class GetAdminByAccountNumberQueryHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly GetAdminByAccountNumberQueryHandler _handler;

        public GetAdminByAccountNumberQueryHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new GetAdminByAccountNumberQueryHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_GivenValidAccountNumber_ShouldReturnAdmin()
        {
            // Arrange
            var accountNumber = 1234567890L;
            var admin = new Admin { AccountNumber = accountNumber, Email = "admin@example.com" };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> {admin});
            
            _contextMock.Setup(c => c.Admins).Returns(admins);

            var query = new GetAdminByAccountNumberQuery { AccountNumber = accountNumber };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccountNumber.Should().Be(accountNumber);
            result.Email.Should().Be(admin.Email);
        }

        [Fact]
        public async Task Handle_GivenInvalidAccountNumber_ShouldReturnNull()
        {
            // Arrange
            var accountNumber = 1234567890L;
            var admin = new Admin { AccountNumber = accountNumber, Email = "admin@example.com" };
            var admins= ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> {admin});

            _contextMock.Setup(c => c.Admins).Returns(admins);

            var query = new GetAdminByAccountNumberQuery { AccountNumber = 2345671234L };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
