
using Application.Admins.Queries;
using Application.Interfaces;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Admins.Queries
{
    public class GetAdminByIdQueryHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly GetAdminByIdQueryHandler _handler;

        public GetAdminByIdQueryHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new GetAdminByIdQueryHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_GivenValidAdminId_ShouldReturnSuccessResult()
        {
            // Arrange
            var adminId = 1;
            var admin = new Admin { Id = adminId, Email = "admin@example.com" };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin> {admin});

            _contextMock.Setup(c => c.Admins).Returns(admins);
            _contextMock.Setup(c => c.Admins.FindAsync(adminId)).ReturnsAsync(admin);

            var query = new GetAdminByIdQuery { Id = adminId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(admin);
            result.Message.Should().Be("Admin found.");
        }

        [Fact]
        public async Task Handle_GivenInvalidAdminId_ShouldReturnFailureResult()  
        {  
            // Arrange  
            var invalidAdminId = 999; 
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin>());  

            _contextMock.Setup(c => c.Admins).Returns(admins);  

            var query = new GetAdminByIdQuery { Id = invalidAdminId };  

            // Act  
            var result = await _handler.Handle(query, CancellationToken.None);  

            // Assert  
            result.Succeeded.Should().BeFalse();  
            result.Message.Should().Be($"Admin with ID {invalidAdminId} not found.");  
        }
    }
}