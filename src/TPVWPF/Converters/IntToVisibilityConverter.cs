using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TPVWPF.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                if (parameter as string == "NotZero")
                {
                    return count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}