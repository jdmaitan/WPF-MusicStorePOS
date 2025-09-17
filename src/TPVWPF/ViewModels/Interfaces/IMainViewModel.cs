using System.ComponentModel;
using System.Windows.Input;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface IMainViewModel
    {
        object CurrentView { get; set; }
        ICommand ShowCartCommand { get; }
        ICommand ShowCatalogCommand { get; }
        ICommand ShowProductAdminCommand { get; }
        ICommand ShowReportsCommand { get; }
        ICommand ShowSalesListCommand { get; }
        ICommand ShowSettingsCommand { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}