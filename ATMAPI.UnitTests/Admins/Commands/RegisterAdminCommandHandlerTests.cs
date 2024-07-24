using Application.Admins.Commands;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MockQueryable.Moq;
using Moq;
using ATMAPI.UnitTests.Data;
using ATMAPI.UnitTests.Mocks;


namespace ATMAPI.UnitTests.Admins.Commands
{
    public class RegisterAdminCommandHandlerTests
    {
        private readonly Mock<IValidator<RegisterAdminCommand>> _validatorMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IAccountFactory> _accountFactoryMock;
        private readonly Mock<IDataContext> _contextMock;
        private readonly RegisterAdminCommandHandler _handler;

        public RegisterAdminCommandHandlerTests()
        {
            _validatorMock = new Mock<IValidator<RegisterAdminCommand>>();
            _emailSenderMock = new Mock<IEmailSender>();
            _accountFactoryMock = new Mock<IAccountFactory>();
            _contextMock = new Mock<IDataContext>();
            _handler = new RegisterAdminCommandHandler(_contextMock.Object, _validatorMock.Object, _emailSenderMock.Object, _accountFactoryMock.Object);
            
            
            // Setup mock behavior for _contextMock if needed
            var admins = ApplicationDbContextText.GetQueryableMockDbSet(Faker.GenerateAdmins());
            _contextMock.Setup(x => x.Admins).Returns(admins);
        }
        
        [InlineData(10)] // Change the number for desired iterations
        [Theory]
        public async Task Handle_Valid_ReturnSuccess(int iterations)
        {
            // Arrange
            for (int i = 0; i < iterations; i++)
            {
                // Generate unique admin data for each iteration
                var expectedAdmin = Faker.GenerateAdmin();
                var command = new RegisterAdminCommand
                {
                    Email = expectedAdmin.Email,
                    Password = expectedAdmin.Password,
                    FirstName = expectedAdmin.FirstName,
                    LastName = expectedAdmin.LastName,
                    Pin = expectedAdmin.Pin
                };
                

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Succeeded.Should().BeTrue();
                result.Message.Should().NotBeNull(); 
                result.Message.Should().Contain("Admin registered successfully.");
                _contextMock.Verify(c => c.Admins.AddAsync(It.IsAny<Admin>(), It.IsAny<CancellationToken>()), Times.Once);
                _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task Handle_GivenInvalidCommand_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new RegisterAdminCommand
            {
                Email = "invalidemail",
                Password = "short",
                FirstName = "John123",
                LastName = "Doe@",
                Pin = 123
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Invalid email format"),
                new ValidationFailure("Password", "Password must be at least 8 characters long, contain at least one number and one special character"),
                new ValidationFailure("FirstName", "First name contains invalid character"),
                new ValidationFailure("LastName", "Last name contains invalid character"),
                new ValidationFailure("Pin", "Invalid PIN length. PIN must be 4 digits")
            };
            _validatorMock.Setup(v => v.Validate(It.IsAny<RegisterAdminCommand>())).Returns(new ValidationResult(validationFailures));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().NotBeNull(); // Ensure message is not null
            result.Message.Should().Contain("Invalid email format");
            result.Message.Should().Contain("Password must be at least 8 characters long, contain at least one number and one special character");
            result.Message.Should().Contain("First name contains invalid character");
            result.Message.Should().Contain("Last name contains invalid character");
            result.Message.Should().Contain("Invalid PIN length. PIN must be 4 digits");
        }

        [Fact]
        public async Task Handle_GivenExistingEmail_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new RegisterAdminCommand { Email = "test@example.com" };
            _validatorMock.Setup(v => v.Validate(It.IsAny<RegisterAdminCommand>())).Returns(new ValidationResult());

            var existingAdmins = ApplicationDbContextText.GetQueryableMockDbSet( new List<Admin> { new Admin { Email = "test@example.com" } });
            _contextMock.Setup(c => c.Admins).Returns(existingAdmins);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().NotBeNull(); // Ensure message is not null
            result.Message.Should().Contain("Email is registered already");
        }

        // [Fact]
        // public async Task Handle_GivenValidCommand_ShouldReturnSuccessResult()
        // {
        //     // Arrange
        //     var command = new RegisterAdminCommand
        //     {
        //         Email = "test@example.com",
        //         Password = "Password@123",
        //         FirstName = "FirstName",
        //         LastName = "LastName",
        //         Pin = 1234
        //     };
        //
        //     _validatorMock.Setup(v => v.Validate(It.IsAny<RegisterAdminCommand>())).Returns(new ValidationResult());
        //
        //     var emptyAdmins = new List<Admin>().AsQueryable().BuildMockDbSet();
        //     _contextMock.Setup(c => c.Admins).Returns(emptyAdmins.Object);
        //     var newAdmin = new Admin { Email = command.Email, FirstName = command.FirstName, LastName = command.LastName, Pin = command.Pin };
        //     _accountFactoryMock.Setup(a => a.CreateAccount<Admin>(command.Email, command.Password, command.FirstName, command.LastName, command.Pin, "Admin")).Returns(newAdmin);
        //
        //     // Act
        //     var result = await _handler.Handle(command, CancellationToken.None);
        //
        //     // Assert
        //     result.Succeeded.Should().BeTrue();
        //     result.Message.Should().NotBeNull(); // Ensure message is not null
        //     result.Message.Should().Contain("Admin registered successfully.");
        //     _contextMock.Verify(c => c.Admins.AddAsync(It.IsAny<Admin>(), It.IsAny<CancellationToken>()), Times.Once);
        //     _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // }

    }
}
