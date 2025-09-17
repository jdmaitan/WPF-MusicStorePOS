using System.ComponentModel;

namespace TPVWPF.ViewModels.Interfaces
{
    public interface IReportsViewModel
    {
        decimal MonthlyRevenue { get; set; }
        int TotalProducts { get; set; }
        decimal TotalRevenue { get; set; }
        int TotalSales { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}