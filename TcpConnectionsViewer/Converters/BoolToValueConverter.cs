using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    /// <summary>
    /// http://geekswithblogs.net/codingbloke/archive/2010/05/28/a-generic-boolean-value-converter.aspx
    /// </summary>
    internal class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return DependencyProperty.UnsetValue;

            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
}
