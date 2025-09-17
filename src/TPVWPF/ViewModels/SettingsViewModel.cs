using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Properties;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class LanguageOption
    {
        public string Name { get; set; }  // Texto visible: "Español"
        public string Code { get; set; }  // Valor real: "es-ES"
    }


    public class SettingsViewModel : INotifyPropertyChanged, ISettingsViewModel
    {
        private string _selectedLanguage;
        private readonly LocalizationService _localizationService;

        public SettingsViewModel(LocalizationService localizationService)
        {
            LoggingService.Information("SettingsViewModel inicializado.");
            _localizationService = localizationService;

            AvailableLanguages = new ObservableCollection<LanguageOption>
            {
                new LanguageOption { Name = "Español", Code = "es-ES" },
                new LanguageOption { Name = "Inglés", Code = "en-US" }
            };
            _selectedLanguage = Settings.Default.PreferredLanguage ?? "es-ES"; // Cargar la preferencia guardada o usar español por defecto
            LoggingService.Debug("Idioma preferido cargado al inicio: {PreferredLanguage}", _selectedLanguage);
            SaveLanguageCommand = new RelayCommand(SaveLanguage);
        }

        public ObservableCollection<LanguageOption> AvailableLanguages { get; }


        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
                LoggingService.Debug("Idioma seleccionado cambiado a: {SelectedLanguageCode}", _selectedLanguage);
            }
        }

        public ICommand SaveLanguageCommand { get; }

        private void SaveLanguage(object parameter)
        {
            LoggingService.Information("Guardando la preferencia de idioma. Idioma seleccionado: {SelectedLanguageCode}", SelectedLanguage);
            if (string.IsNullOrEmpty(SelectedLanguage))
            {
                SelectedLanguage = "es-ES";
                LoggingService.Warning("El idioma seleccionado estaba vacío. Se estableció el idioma por defecto: es-ES.");
            }

            Settings.Default.PreferredLanguage = SelectedLanguage;
            Settings.Default.Save();
            LoggingService.Debug("Preferencia de idioma guardada en la configuración: {SavedLanguageCode}", SelectedLanguage);

            try
            {
                _localizationService.CurrentCulture = new CultureInfo(SelectedLanguage); // Usar el servicio para cambiar la cultura
                LoggingService.Information("Cultura de la aplicación actualizada a: {CurrentCulture}", _localizationService.CurrentCulture.Name);
            }
            catch (CultureNotFoundException)
            {
                _localizationService.CurrentCulture = new CultureInfo("es-ES");
                LoggingService.Error("Cultura '{SelectedLanguageCode}' no encontrada. Se estableció la cultura por defecto: es-ES.", SelectedLanguage);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}