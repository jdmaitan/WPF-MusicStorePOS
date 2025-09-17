using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Models;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class ProductDetailViewModel : INotifyPropertyChanged, IProductDetailViewModel
    {
        private Product _product;
        private readonly CartViewModel _cartViewModel;

        public ProductDetailViewModel(CartViewModel cartViewModel)
        {
            LoggingService.Information("ProductDetailViewModel inicializado.");
            _cartViewModel = cartViewModel;
            AddToCartCommand = new RelayCommand(AddToCart);
        }

        public Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
                if (_product != null)
                {
                    LoggingService.Debug("Detalles del producto mostrados: {ProductName} (ID: {ProductId})", _product.Name, _product.Id);
                }
                else
                {
                    LoggingService.Debug("Ningún producto seleccionado para mostrar detalles.");
                }
            }
        }

        public ICommand AddToCartCommand { get; }

        private void AddToCart(object parameter)
        {
            if (parameter is Product productToAdd)
            {
                LoggingService.Information("El usuario ha intentado agregar el producto '{ProductName}' (ID: {ProductId}) al carrito desde la vista de detalles.", productToAdd.Name, productToAdd.Id);
                _cartViewModel.AddProduct(productToAdd);
                LoggingService.Debug("Producto '{ProductName}' (ID: {ProductId}) agregado al carrito desde la vista de detalles.", productToAdd.Name, productToAdd.Id);
                System.Windows.MessageBox.Show($"Product '{productToAdd.Name}' added to cart.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}