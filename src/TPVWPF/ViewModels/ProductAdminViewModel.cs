using Serilog;
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
    public class ProductAdminViewModel : INotifyPropertyChanged, IProductAdminViewModel
    {
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private readonly ApplicationDbContext _dbContext;

        public ProductAdminViewModel(ApplicationDbContext dbContext)
        {
            LoggingService.Information("ProductAdminViewModel inicializado.");
            _dbContext = dbContext;
            LoadProducts();
            AddNewProductCommand = new RelayCommand(AddNewProduct);
            EditSelectedProductCommand = new RelayCommand(EditSelectedProduct, CanEditOrDeleteProduct);
            DeleteSelectedProductCommand = new RelayCommand(DeleteSelectedProduct, CanEditOrDeleteProduct);
        }

        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
                LoggingService.Debug("Lista de productos en administración actualizada. Nuevo conteo: {ProductCount}", _products?.Count);
            }
        }

        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                // Notificar que la propiedad IsProductSelected ha cambiado para actualizar el estado de los botones
                OnPropertyChanged(nameof(IsProductSelected));
                if (_selectedProduct != null)
                {
                    LoggingService.Debug("Producto seleccionado para administración: {ProductName} (ID: {ProductId})", _selectedProduct.Name, _selectedProduct.Id);
                }
                else
                {
                    LoggingService.Debug("Ningún producto seleccionado en la administración.");
                }
            }
        }

        public bool IsProductSelected => SelectedProduct != null;

        public ICommand AddNewProductCommand { get; }
        public ICommand EditSelectedProductCommand { get; }
        public ICommand DeleteSelectedProductCommand { get; }

        public event Action RequestNewProductView;
        public event Action<Product> RequestEditProductView;

        private void LoadProducts()
        {
            LoggingService.Information("Cargando productos para la administración desde la base de datos.");
            try
            {
                var productsFromDb = _dbContext.Products.ToList();
                Products = new ObservableCollection<Product>(productsFromDb);
                LoggingService.Debug("Se cargaron {ProductCount} productos para la administración.", Products.Count);
            }
            catch (System.Exception ex)
            {
                LoggingService.Error("Error al cargar los productos para la administración desde la base de datos.", ex);
            }
        }

        private void AddNewProduct(object parameter)
        {
            LoggingService.Information("Solicitando la vista para agregar un nuevo producto.");
            RequestNewProductView?.Invoke();
        }

        private void EditSelectedProduct(object parameter)
        {
            if (SelectedProduct != null)
            {
                LoggingService.Information("Solicitando la vista para editar el producto: {ProductName} (ID: {ProductId}).", SelectedProduct.Name, SelectedProduct.Id);
                RequestEditProductView?.Invoke(SelectedProduct);
            }
        }

        private bool CanEditOrDeleteProduct(object parameter)
        {
            bool can = SelectedProduct != null;
            LoggingService.Debug("¿Se puede editar o eliminar el producto seleccionado? {CanEditOrDelete}", can);
            return can;
        }

        private void DeleteSelectedProduct(object parameter)
        {
            if (SelectedProduct != null)
            {
                LoggingService.Warning("Se ha solicitado la eliminación del producto: {ProductName} (ID: {ProductId}).", SelectedProduct.Name, SelectedProduct.Id);
                try
                {
                    _dbContext.Products.Remove(SelectedProduct);
                    _dbContext.SaveChanges();
                    Products.Remove(SelectedProduct); // Actualizar la lista en la vista

                    Log.Information("Producto '{ProductName}' (ID: {ProductId}) eliminado exitosamente.", SelectedProduct?.Name, SelectedProduct?.Id);

                    SelectedProduct = null; // Deseleccionar el producto después de eliminarlo
                }
                catch (System.Exception ex)
                {
                    LoggingService.Error("Error al eliminar el producto: {ProductName} (ID: {ProductId}).", SelectedProduct?.Name, SelectedProduct?.Id, ex);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}