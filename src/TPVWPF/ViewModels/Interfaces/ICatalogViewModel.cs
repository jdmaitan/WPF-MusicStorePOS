using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface ICatalogViewModel
    {
        ICommand AddToCartCommand { get; }
        ObservableCollection<Product> Products { get; set; }
        Product SelectedProduct { get; set; }
        ICommand ShowProductDetailsCommand { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}