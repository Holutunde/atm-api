using Application.Admins.Commands;
using Application.Common.ResultsModel;
using Application.Interfaces;
using ATMAPI.UnitTests.Data;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;


namespace ATMAPI.UnitTests.Admins.Commands
{
    public class LoginAdminCommandHandlerTests
    {
        private readonly Mock<IDataContext> _contextMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly LoginAdminCommandHandler _handler;

        public LoginAdminCommandHandlerTests() 
        {
            _contextMock = new Mock<IDataContext>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _handler = new LoginAdminCommandHandler(_contextMock.Object, _jwtTokenServiceMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task Handle_GivenInvalidEmail_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new LoginAdminCommand { Email = "nonexistent@example.com", Password = "password" };
            var admins =  ApplicationDbContextText.GetQueryableMockDbSet(new List<Admin>());
            _contextMock.Setup(c => c.Admins).Returns(admins);

            //   
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("Invalid credentials.");
        }

        [Fact]
        public async Task Handle_GivenInvalidPassword_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new LoginAdminCommand { Email = "test@example.com", Password = "wrongpassword" };
            var admin = new Admin { Email = "test@example.com", Password = "hashedpassword" };
            var admins =  ApplicationDbContextText.GetQueryableMockDbSet( new List<Admin> { admin });
            _contextMock.Setup(c => c.Admins).Returns(admins);
            _passwordHasherMock.Setup(p => p.VerifyPassword(command.Password, admin.Password)).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Contain("Invalid credentials.");
        }

        [Fact]
        public async Task Handle_GivenValidCredentials_ShouldReturnSuccessResult()
        {
            //var newAdmin = Faker.GenerateAdmin();
           
            // Arrange
            var command = new LoginAdminCommand { Email = "test@example.com", Password = "correctpassword" };
            var admin = new Admin { Email = "test@example.com", Password = "hashedpassword", Role = "Admin" };
            var admins = ApplicationDbContextText.GetQueryableMockDbSet( new List<Admin> { admin });
            _contextMock.Setup(c => c.Admins).Returns(admins);
            _passwordHasherMock.Setup(p => p.VerifyPassword(command.Password, admin.Password)).Returns(true);
            _jwtTokenServiceMock.Setup(j => j.GenerateToken(admin.Email, admin.Role)).Returns("validtoken");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Login successful.");
        }
    }
}
