using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.Models.Cart;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class CheckoutViewModel : INotifyPropertyChanged, ICheckoutViewModel
    {
        private readonly ICartViewModel _cartViewModel;
        private readonly IApplicationDbContext _dbContext;
        private string _customerFirstName;
        private string _customerLastName;
        private string _customerAddress;
        private string _selectedPaymentMethod;
        private ObservableCollection<string> _paymentMethods;

        public CheckoutViewModel(ICartViewModel cartViewModel)
        {
            LoggingService.Information("CheckoutViewModel inicializado. Total del carrito al iniciar el checkout: {Total}", cartViewModel.Total);
            _cartViewModel = cartViewModel;
            _dbContext = new ApplicationDbContext();
            PaymentMethods = new ObservableCollection<string> { "Efectivo", "Tarjeta de Crédito", "Tarjeta de Débito" };
            FinalizeOrderCommand = new RelayCommand(FinalizeOrder);
        }

        public ObservableCollection<CartItem> Items => _cartViewModel.Items;
        public decimal Total => _cartViewModel.Total;
        public string FormattedTotal => $"Total a Pagar: {Total:C2}";

        public string CustomerFirstName
        {
            get { return _customerFirstName; }
            set
            {
                _customerFirstName = value;
                OnPropertyChanged(nameof(CustomerFirstName));
                LoggingService.Debug("Nombre del cliente ingresado: {FirstName}", _customerFirstName);
            }
        }

        public string CustomerLastName
        {
            get { return _customerLastName; }
            set
            {
                _customerLastName = value;
                OnPropertyChanged(nameof(CustomerLastName));
                LoggingService.Debug("Apellido del cliente ingresado: {LastName}", _customerLastName);
            }
        }

        public string CustomerAddress
        {
            get { return _customerAddress; }
            set
            {
                _customerAddress = value;
                OnPropertyChanged(nameof(CustomerAddress));
                LoggingService.Debug("Dirección del cliente ingresada: {Address}", _customerAddress);
            }
        }

        public ObservableCollection<string> PaymentMethods
        {
            get { return _paymentMethods; }
            set { _paymentMethods = value; OnPropertyChanged(nameof(PaymentMethods)); }
        }

        public string SelectedPaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set
            {
                _selectedPaymentMethod = value;
                OnPropertyChanged(nameof(SelectedPaymentMethod));
                LoggingService.Debug("Método de pago seleccionado: {PaymentMethod}", _selectedPaymentMethod);
            }
        }

        public ICommand FinalizeOrderCommand { get; }

        public event Action RequestNavigationToCatalog;


        private void FinalizeOrder(object parameter)
        {
            LoggingService.Information("El usuario ha intentado finalizar la orden. Total a pagar: {Total}, Método de pago: {PaymentMethod}", Total, SelectedPaymentMethod);

            if (string.IsNullOrEmpty(CustomerFirstName) || string.IsNullOrEmpty(CustomerLastName) || string.IsNullOrEmpty(CustomerAddress) || string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                LoggingService.Warning("Faltan datos del cliente para finalizar la compra.");
                MessageBox.Show("Por favor, complete todos los datos del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                //Crear un nuevo Customer
                var newCustomer = new Customer
                {
                    FirstName = CustomerFirstName,
                    LastName = CustomerLastName,
                    Address = CustomerAddress,
                    PaymentMethod = SelectedPaymentMethod
                };
                _dbContext.Customers.Add(newCustomer);
                _dbContext.SaveChanges(); // Guardar el cliente para obtener el ID
                LoggingService.Debug("Nuevo cliente creado con ID: {CustomerId}", newCustomer.Id);


                //Crear un nuevo Ticket
                var newTicket = new Ticket
                {
                    CustomerId = newCustomer.Id,
                    Date = DateTime.Now,
                    Total = Total,
                    TicketLines = Items.Select(item => new TicketLine
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        Subtotal = item.Subtotal
                    }).ToList()
                };

                _dbContext.Tickets.Add(newTicket);
                _dbContext.SaveChanges(); // Guardar el ticket para obtener el ID
                LoggingService.Debug("Nuevo ticket creado con ID: {TicketId} para el cliente {CustomerId}.", newTicket.Id, newCustomer.Id);

                //Vaciar el carrito
                _cartViewModel.Items.Clear();
                LoggingService.Debug("Carrito de compras vaciado después de la compra.");

                //Establecer el Total del CartViewModel a 0
                _cartViewModel.Total = 0;
                LoggingService.Debug("Total del carrito restablecido a 0.");

                //Mostrar mensaje de éxito
                MessageBox.Show("¡Compra realizada con éxito!", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
                LoggingService.Information("Compra finalizada con éxito. Número de ticket: {TicketId}", newTicket.Id);

                //Navegar de vuelta al catálogo
                RequestNavigationToCatalog?.Invoke();
                LoggingService.Debug("Solicitando navegación de vuelta al catálogo.");

            }
            catch (Exception ex)
            {
                LoggingService.Error("Error al finalizar la compra.", ex);
                MessageBox.Show($"Error al finalizar la compra: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}