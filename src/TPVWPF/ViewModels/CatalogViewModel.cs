using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;
using System.Windows.Data;

namespace TPVWPF.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged, ICatalogViewModel
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICartViewModel _cartViewModel;
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private string _searchText;
        private ICollectionView _productsView;

        public CatalogViewModel(IApplicationDbContext dbContext, ICartViewModel cartViewModel)
        {
            LoggingService.Information("CatalogViewModel inicializado.");
            _dbContext = dbContext;
            _cartViewModel = cartViewModel;
            LoadProducts();
            ShowProductDetailsCommand = new RelayCommand(ShowProductDetails);
            AddToCartCommand = new RelayCommand(AddToCart);
        }

        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
                LoggingService.Debug("Lista de productos actualizada. Nuevo conteo: {ProductCount}", _products?.Count);

                // Inicializar la vista de colección para filtrado
                _productsView = CollectionViewSource.GetDefaultView(_products);
                _productsView.Filter = FilterProducts;
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                // Refrescar el filtro cuando cambia el texto de búsqueda
                _productsView?.Refresh();
            }
        }

        private bool FilterProducts(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is Product product)
            {
                return product.Name.ToLower().Contains(SearchText.ToLower());
            }

            return false;
        }

        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                if (_selectedProduct != null)
                {
                    LoggingService.Debug("Producto seleccionado en el catálogo: {ProductName} (ID: {ProductId})", _selectedProduct.Name, _selectedProduct.Id);
                }
            }
        }

        public ICommand ShowProductDetailsCommand { get; set; }
        public ICommand AddToCartCommand { get; }

        private void LoadProducts()
        {
            LoggingService.Information("Cargando productos desde la base de datos.");
            try
            {
                Products = new ObservableCollection<Product>(_dbContext.Products.ToList());
                LoggingService.Debug("Se cargaron {ProductCount} productos.", Products.Count);
            }
            catch (System.Exception ex)
            {
                LoggingService.Error("Error al cargar los productos desde la base de datos.", ex);
            }
        }

        private void ShowProductDetails(object parameter)
        {
            if (parameter is Product product)
            {
                LoggingService.Information("Solicitando mostrar detalles del producto: {ProductName} (ID: {ProductId})", product.Name, product.Id);
            }
        }

        private void AddToCart(object parameter)
        {
            if (parameter is Product product)
            {
                LoggingService.Information("El usuario ha intentado agregar el producto '{ProductName}' (ID: {ProductId}) al carrito desde el catálogo.", product.Name, product.Id);
                _cartViewModel.AddProduct(product);
                LoggingService.Debug("Producto '{ProductName}' (ID: {ProductId}) agregado al carrito.", product.Name, product.Id);
                System.Windows.MessageBox.Show($"Producto '{product.Name}' agregado al carrito.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}