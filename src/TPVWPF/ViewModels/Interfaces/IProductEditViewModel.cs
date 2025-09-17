using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface IProductEditViewModel
    {
        ICommand CancelCommand { get; }
        Product Product { get; set; }
        ICommand SaveCommand { get; }

        event PropertyChangedEventHandler PropertyChanged;
        event Action RequestCloseView;
    }
}