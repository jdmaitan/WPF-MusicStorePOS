using System.ComponentModel;
using System.Windows.Input;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface ILoginViewModel
    {
        string ErrorMessage { get; set; }
        ICommand LoginCommand { get; }
        string Password { get; set; }
        string Username { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}