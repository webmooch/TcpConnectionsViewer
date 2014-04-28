using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(TcpState), typeof(bool))]
    internal class TcpStateEqualsEstablishedToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = value as TcpState?;
            if (state == null)
                throw new ArgumentException("Value must be of type TcpState");

            return state == TcpState.Established;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
