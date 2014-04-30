using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(int), typeof(bool), ParameterType = typeof(int))]
    internal class IntIsGreaterThanToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue;
            int intParam;

            if (!int.TryParse(string.Format("{0}", value), out intValue))
                return DependencyProperty.UnsetValue;

            if (!int.TryParse(string.Format("{0}", parameter), out intParam))
                return DependencyProperty.UnsetValue;

            return intValue > intParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
