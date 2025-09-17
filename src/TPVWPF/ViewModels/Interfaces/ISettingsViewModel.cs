using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface ISettingsViewModel
    {
        ObservableCollection<LanguageOption> AvailableLanguages { get; }
        ICommand SaveLanguageCommand { get; }
        string SelectedLanguage { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}