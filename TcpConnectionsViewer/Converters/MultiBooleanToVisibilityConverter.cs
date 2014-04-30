using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    internal class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value == DependencyProperty.UnsetValue)
                    return DependencyProperty.UnsetValue;

                if (!(value is bool))
                    return DependencyProperty.UnsetValue;

                if (!(bool)value)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}