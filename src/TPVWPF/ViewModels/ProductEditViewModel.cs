using System.ComponentModel;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class ProductEditViewModel : INotifyPropertyChanged, IProductEditViewModel
    {
        private Product _product;
        private readonly ApplicationDbContext _dbContext;
        private bool _isNewProduct; // Para rastrear si se está creando un nuevo producto

        // Constructor para la creación de un nuevo producto
        public ProductEditViewModel(ApplicationDbContext dbContext)
        {
            LoggingService.Information("ProductEditViewModel inicializado para la creación de un nuevo producto.");
            _dbContext = dbContext;
            Product = new Product();
            _isNewProduct = true;
            SaveCommand = new RelayCommand(SaveProduct);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        // Constructor para la edición de un producto existente
        public ProductEditViewModel(ApplicationDbContext dbContext, Product productToEdit)
        {
            LoggingService.Information("ProductEditViewModel inicializado para la edición del producto: {ProductName} (ID: {ProductId}).", productToEdit.Name, productToEdit.Id);
            _dbContext = dbContext;
            Product = productToEdit;
            _isNewProduct = false;
            SaveCommand = new RelayCommand(SaveProduct);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
                LoggingService.Debug("Datos del producto en edición/creación actualizados: {ProductName} (ID: {ProductId}, Precio: {Price:C2}, Categoría: {Category})",
                                       _product.Name, _product.Id, _product.Price, _product.Category);
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action RequestCloseView;

        private void SaveProduct(object parameter)
        {
            if (_isNewProduct)
            {
                LoggingService.Information("Guardando un nuevo producto: {ProductName} (Precio: {Price:C2}, Categoría: {Category}).", Product.Name, Product.Price, Product.Category);
                _dbContext.Products.Add(Product);
            }
            else
            {
                LoggingService.Information("Guardando los cambios del producto existente: {ProductName} (ID: {ProductId}, Precio: {Price:C2}, Categoría: {Category}).", Product.Name, Product.Id, Product.Price, Product.Category);
                _dbContext.Products.Update(Product);
            }

            try
            {
                _dbContext.SaveChanges();
                LoggingService.Information("Producto '{ProductName}' (ID: {ProductId}) guardado exitosamente.", Product.Name, Product.Id);
                RequestCloseView?.Invoke(); // Notificar al MainViewModel que cierre la vista
            }
            catch (System.Exception ex)
            {
                LoggingService.Error("Error al guardar el producto '{ProductName}' (ID: {ProductId}).", Product.Name, Product.Id, ex);
            }
        }

        private void CancelEdit(object parameter)
        {
            LoggingService.Information("Edición/creación de producto cancelada.");
            RequestCloseView?.Invoke(); // Notificar al MainViewModel que cierre la vista
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}