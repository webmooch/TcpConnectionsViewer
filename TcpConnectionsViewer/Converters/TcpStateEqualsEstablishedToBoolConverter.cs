using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(TcpState), typeof(bool))]
    internal class TcpStateEqualsEstablishedToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TcpState))
                return DependencyProperty.UnsetValue;

            return (TcpState)value == TcpState.Established;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
