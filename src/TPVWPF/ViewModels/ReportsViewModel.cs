using System.ComponentModel;
using TPVWPF.Data;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged, IReportsViewModel
    {
        private int _totalProducts;
        private int _totalSales;
        private decimal _totalRevenue;
        private decimal _monthlyRevenue;
        private readonly ApplicationDbContext _dbContext;

        public ReportsViewModel(ApplicationDbContext dbContext)
        {
            LoggingService.Information("ReportsViewModel inicializado.");
            _dbContext = dbContext;
            LoadReports();
        }

        public int TotalProducts
        {
            get { return _totalProducts; }
            set
            {
                _totalProducts = value;
                OnPropertyChanged(nameof(TotalProducts));
                LoggingService.Debug("Total de productos cargado: {TotalProducts}", _totalProducts);
            }
        }

        public int TotalSales
        {
            get { return _totalSales; }
            set
            {
                _totalSales = value;
                OnPropertyChanged(nameof(TotalSales));
                LoggingService.Debug("Total de ventas cargado: {TotalSales}", _totalSales);
            }
        }

        public decimal TotalRevenue
        {
            get { return _totalRevenue; }
            set
            {
                _totalRevenue = value;
                OnPropertyChanged(nameof(TotalRevenue));
                LoggingService.Debug("Ingresos totales cargados: {TotalRevenue:C2}", _totalRevenue);
            }
        }

        public decimal MonthlyRevenue
        {
            get { return _monthlyRevenue; }
            set
            {
                _monthlyRevenue = value;
                OnPropertyChanged(nameof(MonthlyRevenue));
                LoggingService.Debug("Ingresos del mes actual ({Month}): {MonthlyRevenue:C2}", DateTime.Now.ToString("MMMM"), _monthlyRevenue);
            }
        }

        private void LoadReports()
        {
            LoggingService.Information("Cargando los datos para los reportes.");

            TotalProducts = _dbContext.Products.Count();
            TotalSales = _dbContext.Tickets.Count();
            TotalRevenue = _dbContext.Tickets.Sum(t => t.Total);

            LoggingService.Debug("Se encontraron {TotalProducts} productos en la base de datos.", TotalProducts);
            LoggingService.Debug("Se encontraron {TotalSales} ventas en la base de datos.", TotalSales);
            LoggingService.Debug("Los ingresos totales ascienden a: {TotalRevenue:C2}.", TotalRevenue);


            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            MonthlyRevenue = _dbContext.Tickets
                .Where(t => t.Date >= currentMonthStart && t.Date <= currentMonthEnd)
                .Sum(t => t.Total);

            if (MonthlyRevenue == null)
            {
                MonthlyRevenue = 0;
            }

            LoggingService.Debug("Los ingresos del mes de {Month} son: {MonthlyRevenue:C2}.", DateTime.Now.ToString("MMMM"), MonthlyRevenue);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}