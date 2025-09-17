using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Models.Cart;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged, ICartViewModel
    {
        private ObservableCollection<CartItem> _items;
        private decimal _total;
        private ICommand _checkoutCommand;
        private bool _isCartEmpty;

        public CartViewModel()
        {
            LoggingService.Information("CartViewModel inicializado.");
            Items = new ObservableCollection<CartItem>();
            DeleteFromCartCommand = new RelayCommand(DeleteFromCart);
            UpdateQuantityCommand = new RelayCommand(UpdateQuantity);
            CheckoutCommand = new RelayCommand(Checkout, CanCheckout);
            UpdateIsCartEmpty();
            Items.CollectionChanged += (sender, e) =>
            {
                UpdateIsCartEmpty();
                LoggingService.Debug("La colección de Items del carrito ha cambiado. Nuevo conteo: {ItemCount}", Items.Count);
            };
        }

        public ObservableCollection<CartItem> Items
        {
            get { return _items; }
            set
            {
                LoggingService.Debug("Se ha asignado una nueva colección de Items al carrito.");
                _items = value;
                OnPropertyChanged(nameof(Items));
                UpdateTotal();
                UpdateIsCartEmpty();
            }
        }

        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
                LoggingService.Debug("Total del carrito actualizado a: {Total}", _total);
            }
        }

        public ICommand DeleteFromCartCommand { get; }
        public ICommand UpdateQuantityCommand { get; }
        public ICommand CheckoutCommand
        {
            get { return _checkoutCommand; }
            set { _checkoutCommand = value; OnPropertyChanged(nameof(CheckoutCommand)); }
        }
        public bool IsCartEmpty
        {
            get { return _isCartEmpty; }
            set
            {
                _isCartEmpty = value;
                OnPropertyChanged(nameof(IsCartEmpty));
                LoggingService.Debug("Estado del carrito vacío actualizado a: {IsCartEmpty}", _isCartEmpty);
            }
        }

        public event Action RequestCheckout; // Evento para notificar al MainViewModel

        private void Checkout(object parameter)
        {
            LoggingService.Information("El usuario ha iniciado el proceso de checkout. Total del carrito: {Total}", Total);
            RequestCheckout?.Invoke();
        }

        private bool CanCheckout(object parameter)
        {
            bool can = !IsCartEmpty;
            LoggingService.Debug("¿Puede el usuario hacer checkout? {CanCheckout}", can);
            return can;
        }

        private void UpdateIsCartEmpty()
        {
            IsCartEmpty = Items.Count == 0;
            // Necesitamos avisar al Command que su estado de "can execute" ha cambiado
            (CheckoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public virtual void AddProduct(Models.Product product)
        {
            LoggingService.Information("Se ha intentado agregar el producto '{ProductName}' (ID: {ProductId}) al carrito.", product.Name, product.Id);
            var existingItem = Items.FirstOrDefault(item => item.Product.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
                LoggingService.Debug("Cantidad del producto '{ProductName}' (ID: {ProductId}) incrementada a: {Quantity}", product.Name, product.Id, existingItem.Quantity);
            }
            else
            {
                Items.Add(new CartItem { Product = product, Quantity = 1 });
                LoggingService.Debug("Producto '{ProductName}' (ID: {ProductId}) añadido al carrito con cantidad 1.", product.Name, product.Id);
            }
            UpdateTotal();
        }

        public void DeleteFromCart(object parameter)
        {
            if (parameter is CartItem itemToRemove)
            {
                LoggingService.Information("Se ha eliminado el producto '{ProductName}' (ID: {ProductId}) del carrito. Cantidad eliminada: {Quantity}", itemToRemove.Product.Name, itemToRemove.Product.Id, itemToRemove.Quantity);
                Items.Remove(itemToRemove);
                UpdateTotal();
            }
        }

        public void UpdateQuantity(object parameter)
        {
            if (parameter is CartItem updatedItem)
            {
                LoggingService.Information("Se ha intentado actualizar la cantidad del producto '{ProductName}' (ID: {ProductId}) a: {Quantity}", updatedItem.Product.Name, updatedItem.Product.Id, updatedItem.Quantity);
                if (updatedItem.Quantity < 1)
                {
                    LoggingService.Warning("La cantidad del producto '{ProductName}' (ID: {ProductId}) es menor que 1. Se mostrará una advertencia.", updatedItem.Product.Name, updatedItem.Product.Id);
                    MessageBox.Show("La cantidad debe ser al menos 1.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            Total = Items.Sum(item => item.Subtotal);
            LoggingService.Verbose("Recalculando el total del carrito. Nuevo total temporal: {Total}", Total);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}