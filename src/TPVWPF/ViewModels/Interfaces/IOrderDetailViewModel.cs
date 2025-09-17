using System.Collections.ObjectModel;
using System.ComponentModel;
using TPVWPF.Models;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface IOrderDetailViewModel
    {
        Ticket CurrentOrder { get; set; }
        Customer Customer { get; set; }
        ObservableCollection<TicketLine> OrderItems { get; set; }
        decimal Total { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}