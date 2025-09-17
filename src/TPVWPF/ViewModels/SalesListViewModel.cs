using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class SalesListViewModel : INotifyPropertyChanged, ISalesListViewModel
    {
        private ObservableCollection<Ticket> _sales;
        private Ticket _selectedSale;
        private readonly ApplicationDbContext _dbContext;

        public SalesListViewModel(ApplicationDbContext dbContext)
        {
            LoggingService.Information("SalesListViewModel inicializado.");
            _dbContext = dbContext;
            LoadSales();
            ShowOrderDetailCommand = new RelayCommand(ShowOrderDetail);
        }

        public ObservableCollection<Ticket> Sales
        {
            get { return _sales; }
            set
            {
                _sales = value;
                OnPropertyChanged(nameof(Sales));
                LoggingService.Debug("Lista de ventas actualizada. Nuevo conteo: {SalesCount}", _sales?.Count);
            }
        }

        public Ticket SelectedSale
        {
            get { return _selectedSale; }
            set
            {
                _selectedSale = value;
                OnPropertyChanged(nameof(SelectedSale));
                if (_selectedSale != null)
                {
                    LoggingService.Debug("Venta seleccionada para ver detalles. ID de la venta: {TicketId}", _selectedSale.Id);
                    ShowOrderDetailCommand.Execute(_selectedSale);
                }
                else
                {
                    LoggingService.Debug("Ninguna venta seleccionada.");
                }
            }
        }

        public ICommand ShowOrderDetailCommand { get; }

        public event Action<Ticket> RequestShowOrderDetail;

        private void ShowOrderDetail(object parameter)
        {
            if (parameter is Ticket selectedTicket)
            {
                LoggingService.Information("Solicitando mostrar detalles de la venta con ID: {TicketId}.", selectedTicket.Id);
                RequestShowOrderDetail?.Invoke(selectedTicket);
            }
        }

        private void LoadSales()
        {
            LoggingService.Information("Cargando la lista de ventas desde la base de datos.");
            try
            {
                // Cargar todos los tickets desde la base de datos, incluyendo la información del cliente
                var ticketsFromDb = _dbContext.Tickets.Include(t => t.Customer).ToList();
                Sales = new ObservableCollection<Ticket>(ticketsFromDb);
                LoggingService.Debug("Se cargaron {SalesCount} ventas.", Sales.Count);
            }
            catch (Exception ex)
            {
                LoggingService.Error("Error al cargar la lista de ventas desde la base de datos.", ex);
                Sales = new ObservableCollection<Ticket>(); // Asegurar que la lista no sea nula en caso de error
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}