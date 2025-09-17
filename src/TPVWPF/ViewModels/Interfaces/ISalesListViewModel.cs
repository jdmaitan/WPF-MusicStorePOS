using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Models;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface ISalesListViewModel
    {
        ObservableCollection<Ticket> Sales { get; set; }
        Ticket SelectedSale { get; set; }
        ICommand ShowOrderDetailCommand { get; }

        event PropertyChangedEventHandler PropertyChanged;
        event Action<Ticket> RequestShowOrderDetail;
    }
}