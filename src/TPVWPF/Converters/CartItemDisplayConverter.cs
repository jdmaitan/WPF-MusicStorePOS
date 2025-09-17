using System;
using System.Globalization;
using System.Windows.Data;

namespace TPVWPF.Converters
{
    public class CartItemDisplayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 && values[0] is string productName && values[1] is int quantity && values[2] is decimal subtotal)
            {
                return $"{productName} x {quantity} - {subtotal:C2}";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}