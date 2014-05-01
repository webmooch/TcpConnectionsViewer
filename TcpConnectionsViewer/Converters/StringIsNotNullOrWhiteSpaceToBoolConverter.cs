using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    internal class StringIsNotNullOrWhiteSpaceToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;

            if (value != null && str == null)
                return DependencyProperty.UnsetValue;

            return !string.IsNullOrWhiteSpace(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
