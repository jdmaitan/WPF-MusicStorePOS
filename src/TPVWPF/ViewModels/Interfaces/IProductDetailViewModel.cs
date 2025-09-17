using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface IProductDetailViewModel
    {
        ICommand AddToCartCommand { get; }
        Product Product { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}