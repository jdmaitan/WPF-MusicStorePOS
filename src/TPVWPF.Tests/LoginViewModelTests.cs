using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.ViewModels;

namespace TPVWPF.Tests
{
    public class LoginViewModelTests : IDisposable
    {
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly Mock<Action> _navigateMock;
        private readonly LoginViewModel _viewModel;
        private readonly TestPasswordBox _passwordBox;

        public LoginViewModelTests()
        {
            var users = new Mock<DbSet<User>>();
            var data = new List<User>().AsQueryable();

            users.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            users.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            users.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            users.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _dbContextMock = new Mock<ApplicationDbContext>();
            _dbContextMock.Setup(x => x.Users).Returns(users.Object);

            _navigateMock = new Mock<Action>();
            _viewModel = new LoginViewModel(_dbContextMock.Object, _navigateMock.Object);
            _passwordBox = new TestPasswordBox { Password = "test123" };
        }

        [Fact]
        public void Login_WithEmptyCredentials_ShowsErrorMessage()
        {
            // Act
            _viewModel.LoginCommand.Execute(_passwordBox);

            // Assert
            _viewModel.ErrorMessage.Should().Be("Por favor, introduce usuario y contraseña.");
            _navigateMock.Verify(x => x(), Times.Never);
        }

        [Fact]
        public void PropertyChanges_ShouldRaiseNotifications()
        {
            // Arrange
            var monitor = _viewModel.Monitor();

            // Act & Assert
            _viewModel.Username = "test";
            monitor.Should().RaisePropertyChangeFor(x => x.Username);

            _viewModel.Password = "pass";
            monitor.Should().RaisePropertyChangeFor(x => x.Password);

            _viewModel.ErrorMessage = "error";
            monitor.Should().RaisePropertyChangeFor(x => x.ErrorMessage);
        }

        public void Dispose()
        {
        }
    }

    public class TestPasswordBox
    {
        public string Password { get; set; }
    }
}