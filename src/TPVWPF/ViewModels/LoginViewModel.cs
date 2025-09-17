using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, ILoginViewModel
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private readonly ApplicationDbContext _dbContext;
        private readonly Action _navigateToCatalog;

        public LoginViewModel(ApplicationDbContext dbContext, Action navigateToCatalog)
        {
            _dbContext = dbContext;
            _navigateToCatalog = navigateToCatalog;
            LoginCommand = new RelayCommand(Login);
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; }

        private void Login(object parameter)
        {
            ErrorMessage = ""; // Limpiar cualquier mensaje de error previo

            if (string.IsNullOrEmpty(Username) || parameter == null)
            {
                ErrorMessage = "Por favor, introduce usuario y contraseña.";
                return;
            }

            if (!(parameter is System.Windows.Controls.PasswordBox passwordBox))
            {
                ErrorMessage = "Error interno: No se pudo acceder a la contraseña.";
                return;
            }

            var password = passwordBox.Password;

            if (string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Por favor, introduce usuario y contraseña.";
                return;
            }

            var user = _dbContext.Users.SingleOrDefault(u => u.Username == Username);

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                LoggingService.Information("Inicio de sesión exitoso para el usuario: {Username}", Username);
                _navigateToCatalog?.Invoke(); // Navegar a la vista del catálogo
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                LoggingService.Warning("Intento de inicio de sesión fallido para el usuario: {Username}", Username);
            }

        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var computedHash = Convert.ToBase64String(hashedBytes);
                return computedHash == hashedPassword;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}