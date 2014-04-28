using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(int), typeof(bool), ParameterType=typeof(int))]
    internal class IntIsGreaterThanToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue;
            int intParam;

            if (int.TryParse(string.Format("{0}", value), out intValue) && int.TryParse(string.Format("{0}", parameter), out intParam))
                return intValue > intParam;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
