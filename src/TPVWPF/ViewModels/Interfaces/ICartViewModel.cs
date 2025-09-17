using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models;
using TPVWPF.Models.Cart;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface ICartViewModel
    {
        ICommand CheckoutCommand { get; set; }
        ICommand DeleteFromCartCommand { get; }
        bool IsCartEmpty { get; set; }
        ObservableCollection<CartItem> Items { get; set; }
        decimal Total { get; set; }
        ICommand UpdateQuantityCommand { get; }

        event PropertyChangedEventHandler PropertyChanged;
        event Action RequestCheckout;

        void AddProduct(Product product);
        void DeleteFromCart(object parameter);
        void UpdateQuantity(object parameter);
    }
}