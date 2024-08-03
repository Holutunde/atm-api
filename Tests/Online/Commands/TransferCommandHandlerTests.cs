using Application.Interfaces;
using Application.Online.Commands;
using ATMAPI.Tests.Mock;
using ATMAPI.UnitTests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.ResultsModel;
using Xunit;

namespace ATMAPI.Tests.Online.Commands
{
    public class TransferCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IDataContext> _mockDataContext;
        private readonly TransferCommandHandler _handler;

        public TransferCommandHandlerTests()
        {
            _mockUserManager = UserManagerMock.CreateMockUserManager<ApplicationUser>();
            _mockDataContext = new Mock<IDataContext>();
            _handler = new TransferCommandHandler(_mockUserManager.Object, _mockDataContext.Object);
        }

        private void SetupMockDataContext(List<ApplicationUser> users)
        {
            var mockSet = ApplicationDbContextText.GetQueryableMockDbSet(users);
            _mockDataContext.Setup(x => x.Users).Returns(mockSet);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSenderNotFound()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "nonexistent@example.com", ReceiverAccountNumber = 123456789L, Amount = 100.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync((ApplicationUser)null);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Sender not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenReceiverNotFound()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "user@example.com", ReceiverAccountNumber = 123456789L, Amount = 100.0 };
            ApplicationUser sender = new ApplicationUser { Email = command.SenderEmail, Balance = 200.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync(sender);

            SetupMockDataContext(new List<ApplicationUser>());

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Receiver not found.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSenderAndReceiverAreTheSame()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "user@example.com", ReceiverAccountNumber = 12345, Amount = 100.0 };
            ApplicationUser sender = new ApplicationUser { Email = command.SenderEmail, AccountNumber = 12345, Balance = 200.0 };
            ApplicationUser receiver = new ApplicationUser { AccountNumber = command.ReceiverAccountNumber };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync(sender);

            SetupMockDataContext(new List<ApplicationUser> { receiver });

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Sender cannot transfer to sender account");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInsufficientFunds()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "user@example.com", ReceiverAccountNumber = 12345, Amount = 300.0 };
            ApplicationUser sender = new ApplicationUser { Email = command.SenderEmail, Balance = 200.0 };
            ApplicationUser receiver = new ApplicationUser { AccountNumber = command.ReceiverAccountNumber };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync(sender);

            SetupMockDataContext(new List<ApplicationUser> { receiver });

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Insufficient funds.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTransferIsSuccessful()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "user@example.com", ReceiverAccountNumber = 12345, Amount = 100.0 };
            ApplicationUser sender = new ApplicationUser { Email = command.SenderEmail, Balance = 200.0 };
            ApplicationUser receiver = new ApplicationUser { AccountNumber = command.ReceiverAccountNumber, Balance = 300.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync(sender);
            _mockUserManager.Setup(x => x.UpdateAsync(sender)).ReturnsAsync(IdentityResult.Success);

            SetupMockDataContext(new List<ApplicationUser> { receiver });

            _mockDataContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("Transfer successful.");
            result.Entity.Should().Be(100.0); // New balance of sender after successful transfer
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUpdateSenderFails()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "user@example.com", ReceiverAccountNumber = 12345, Amount = 100.0 };
            ApplicationUser sender = new ApplicationUser { Email = command.SenderEmail, Balance = 200.0 };
            ApplicationUser receiver = new ApplicationUser { AccountNumber = command.ReceiverAccountNumber, Balance = 300.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync(sender);
            _mockUserManager.Setup(x => x.UpdateAsync(sender)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            SetupMockDataContext(new List<ApplicationUser> { receiver });

            _mockDataContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Transfer unsuccessful.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSaveChangesFails()
        {
            // Arrange
            TransferCommand command = new TransferCommand { SenderEmail = "user@example.com", ReceiverAccountNumber = 12345, Amount = 100.0 };
            ApplicationUser sender = new ApplicationUser { Email = command.SenderEmail, Balance = 200.0 };
            ApplicationUser receiver = new ApplicationUser { AccountNumber = command.ReceiverAccountNumber, Balance = 300.0 };
            _mockUserManager.Setup(x => x.FindByEmailAsync(command.SenderEmail)).ReturnsAsync(sender);
            _mockUserManager.Setup(x => x.UpdateAsync(sender)).ReturnsAsync(IdentityResult.Success);

            SetupMockDataContext(new List<ApplicationUser> { receiver });

            _mockDataContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            // Act
            Result result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Transfer unsuccessful.");
        }
    }
}
