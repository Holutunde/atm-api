using Application.Admins.Queries;
using Application.Common.ResultsModel;
using Application.Interfaces;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Moq;


namespace ATMAPI.UnitTests.Admins.Queries
{
    public class GetAllAdminsQueryHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly GetAllAdminsQueryHandler _handler;

        public GetAllAdminsQueryHandlerTests()
        {
            _contextMock = new Mock<IDataContext>();
            _handler = new GetAllAdminsQueryHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllAdmins_WhenNoSearchValueProvided()
        {
            // Arrange
            var admins = new List<Admin>
            {
                new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new Admin { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
            };
            var adminsDbSet = ApplicationDbContextText.GetQueryableMockDbSet(admins);

            _contextMock.Setup(c => c.Admins).Returns(adminsDbSet);

            var query = new GetAllAdminsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var resultData = result.Entity as ResultData;
            resultData.Should().NotBeNull();
            resultData.Admins.Should().HaveCount(2);
            resultData.Count.Should().Be(2);
            result.Message.Should().Be("Total admins found: 2");
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredAdmins_WhenSearchValueProvided()
        {
            // Arrange
            var admins = new List<Admin>
            {
                new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new Admin { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
            };
            var adminsDbSet = ApplicationDbContextText.GetQueryableMockDbSet(admins);

            _contextMock.Setup(c => c.Admins).Returns(adminsDbSet);

            var query = new GetAllAdminsQuery { SearchValue = "Jane" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var resultData = result.Entity as ResultData;
            resultData.Should().NotBeNull();
            resultData.Admins.Should().HaveCount(1);
            resultData.Count.Should().Be(1);
            result.Message.Should().Be("Total admins found: 1");
            resultData.Admins.First().FirstName.Should().Be("Jane");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResult_WhenNoAdminsMatchSearchValue()
        {
            // Arrange
            var admins = new List<Admin>
            {
                new Admin { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new Admin { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
            };
            var adminsDbSet = ApplicationDbContextText.GetQueryableMockDbSet(admins);

            _contextMock.Setup(c => c.Admins).Returns(adminsDbSet);

            var query = new GetAllAdminsQuery { SearchValue = "NonExistent" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var resultData = result.Entity as ResultData;
            resultData.Should().NotBeNull();
            resultData.Admins.Should().BeEmpty();
            resultData.Count.Should().Be(0);
            result.Message.Should().Be("Total admins found: 0");
        }
    }
}
