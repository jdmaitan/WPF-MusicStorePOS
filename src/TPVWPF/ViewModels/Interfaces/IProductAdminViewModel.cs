using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface IProductAdminViewModel
    {
        ICommand AddNewProductCommand { get; }
        ICommand DeleteSelectedProductCommand { get; }
        ICommand EditSelectedProductCommand { get; }
        bool IsProductSelected { get; }
        ObservableCollection<Product> Products { get; set; }
        Product SelectedProduct { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
        event Action<Product> RequestEditProductView;
        event Action RequestNewProductView;
    }
}