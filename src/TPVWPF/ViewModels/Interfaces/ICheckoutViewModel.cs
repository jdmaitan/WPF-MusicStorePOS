using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models.Cart;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface ICheckoutViewModel
    {
        string CustomerAddress { get; set; }
        string CustomerFirstName { get; set; }
        string CustomerLastName { get; set; }
        ICommand FinalizeOrderCommand { get; }
        string FormattedTotal { get; }
        ObservableCollection<CartItem> Items { get; }
        ObservableCollection<string> PaymentMethods { get; set; }
        string SelectedPaymentMethod { get; set; }
        decimal Total { get; }

        event PropertyChangedEventHandler PropertyChanged;
        event Action RequestNavigationToCatalog;
    }
}