using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;
using TPVWPF.Views;

namespace TPVWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IMainViewModel
    {
        private object _currentView;
        private readonly ApplicationDbContext _dbContext;
        private readonly LocalizationService _localizationService;
        private CartViewModel _cartViewModel;
        private bool _isLoggedIn = false;

        public MainViewModel(ApplicationDbContext dbContext, LocalizationService localizationService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _cartViewModel = new CartViewModel();
            _cartViewModel.RequestCheckout += ShowCheckout;

            LoggingService.Information("MainViewModel inicializado.");

            // Acción para navegar al catálogo desde el LoginViewModel
            Action navigateToCatalog = () =>
            {
                IsLoggedIn = true;
                ShowCatalog(null);
            };

            // Inicialmente, mostramos la vista de inicio de sesión
            LoginViewModel loginViewModel = new LoginViewModel(_dbContext, navigateToCatalog);
            CurrentView = new LoginView() { DataContext = loginViewModel };
            LoggingService.Information("Vista inicial establecida: {ViewType}", CurrentView.GetType().Name);

            ShowCartCommand = new RelayCommand(ShowCart);
            ShowCatalogCommand = new RelayCommand(ShowCatalog);
            ShowSalesListCommand = new RelayCommand(ShowSalesList);
            ShowProductAdminCommand = new RelayCommand(ShowProductAdmin);
            ShowReportsCommand = new RelayCommand(ShowReports);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
                LoggingService.Information("Navegación a la vista: {ViewType}", _currentView?.GetType().Name);
            }
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public ICommand ShowCartCommand { get; }
        public ICommand ShowCatalogCommand { get; }
        public ICommand ShowSalesListCommand { get; }
        public ICommand ShowProductAdminCommand { get; }
        public ICommand ShowReportsCommand { get; }
        public ICommand ShowSettingsCommand { get; }


        private void ShowCart(object parameter)
        {
            LoggingService.Information("Mostrando la vista del carrito.");
            CurrentView = new CartView() { DataContext = _cartViewModel };
        }

        private void ShowCatalog(object parameter)
        {
            LoggingService.Information("Mostrando la vista del catálogo.");
            CatalogViewModel catalogViewModel = new CatalogViewModel(_dbContext, _cartViewModel);
            catalogViewModel.ShowProductDetailsCommand = new RelayCommand(ShowProductDetails);
            CurrentView = new CatalogView() { DataContext = catalogViewModel };
        }

        private void ShowProductDetails(object parameter)
        {
            if (parameter is Product selectedProduct)
            {
                LoggingService.Information("Mostrando detalles del producto: {ProductName} (ID: {ProductId})", selectedProduct.Name, selectedProduct.Id);
                CurrentView = new ProductDetailView() { DataContext = new ProductDetailViewModel(_cartViewModel) { Product = selectedProduct } };
            }
        }

        private void ShowCheckout()
        {
            LoggingService.Information("Iniciando el proceso de checkout.");
            var checkoutViewModel = new CheckoutViewModel(_cartViewModel);
            checkoutViewModel.RequestNavigationToCatalog += NavigateBackToCatalog;
            CurrentView = new CheckoutView() { DataContext = checkoutViewModel };
        }

        private void ShowSalesList(object parameter)
        {
            LoggingService.Information("Mostrando el historial de ventas.");
            var salesListViewModel = new SalesListViewModel(_dbContext);
            salesListViewModel.RequestShowOrderDetail += ShowOrderDetailView;
            CurrentView = new SalesListView() { DataContext = salesListViewModel };
        }

        private void ShowOrderDetailView(Ticket selectedTicket)
        {
            LoggingService.Information("Mostrando detalles de la orden con ID: {TicketId}", selectedTicket.Id);
            CurrentView = new OrderDetailView()
            {
                DataContext = new OrderDetailViewModel(_dbContext) { CurrentOrder = selectedTicket }
            };
        }

        private void ShowProductAdmin(object parameter)
        {
            LoggingService.Information("Mostrando la vista de administración de productos.");
            var productAdminViewModel = new ProductAdminViewModel(_dbContext);
            productAdminViewModel.RequestNewProductView += ShowNewProductView;
            productAdminViewModel.RequestEditProductView += ShowEditProductView;
            CurrentView = new ProductAdminView() { DataContext = productAdminViewModel };
        }
        private void ShowNewProductView()
        {
            LoggingService.Information("Mostrando la vista para crear un nuevo producto.");
            var productEditViewModel = new ProductEditViewModel(_dbContext);
            productEditViewModel.RequestCloseView += CloseProductEditView;
            CurrentView = new ProductEditView() { DataContext = productEditViewModel };
        }

        private void ShowEditProductView(Product productToEdit)
        {
            LoggingService.Information("Mostrando la vista para editar el producto: {ProductName} (ID: {ProductId})", productToEdit.Name, productToEdit.Id);
            var productEditViewModel = new ProductEditViewModel(_dbContext, productToEdit);
            productEditViewModel.RequestCloseView += CloseProductEditView;
            CurrentView = new ProductEditView() { DataContext = productEditViewModel };
        }

        private void ShowReports(object parameter)
        {
            LoggingService.Information("Mostrando la vista de reportes.");
            CurrentView = new ReportsView() { DataContext = new ReportsViewModel(_dbContext) };
        }

        private void ShowSettings(object parameter)
        {
            LoggingService.Information("Mostrando la vista de configuración.");
            CurrentView = new SettingsView
            {
                DataContext = new SettingsViewModel(_localizationService)
            };
        }

        private void CloseProductEditView()
        {
            LoggingService.Information("Cerrando la vista de edición de productos y volviendo a la administración de productos.");
            // Recargamos la lista de productos al volver a la vista de administración
            ShowProductAdmin(null);
        }

        private void NavigateBackToCatalog()
        {
            LoggingService.Information("Volviendo a la vista del catálogo desde el checkout.");
            ShowCatalog(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}